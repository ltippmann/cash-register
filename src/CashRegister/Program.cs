using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CashRegister.Domain;
using CommandLine;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("CashRegister.UnitTests")]

namespace CashRegister
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineArgs>(args).WithParsed(Run);
        }

        public class CommandLineArgs
        {
            [Value(0, MetaName = "InputFiles", Required = true, HelpText = "Input CSV files to process.")]
            public IEnumerable<string> InputFiles { get; set; }
            [Option('o', "output", Default = "output.txt", HelpText = "File to write processing output to.")]
            public string OutputFile { get; set; } = "output.txt";
            [Option('q', "quiet")]
            public bool Quiet { get; set; } = false;
        }

        public static void Run(CommandLineArgs args)
        {
            if (args.InputFiles?.Any() != true)
            {
                Console.WriteLine("   ERROR: No input file specified.");
                return;
            }

            if (File.Exists(args.OutputFile))
            {
                if (args.Quiet)
                {
                    Console.WriteLine("   ERROR: Output file already exists.");
                    return;
                }

                Console.WriteLine("Output file already exists.  Overwrite (y/n)?");
                if (Console.ReadLine().TrimStart().ToLower() != "y")
                    return;
            }

            var changeTabulator = new ChangeTabulator(
                new Domain.ChangeTabulationSrategies.RandomTabulationStrategy(),
                new Domain.ChangeTabulationSrategies.BigEndianTabulationStrategy());

            using var sw = new StreamWriter(args.OutputFile);
            
            foreach (var inputFile in args.InputFiles)
            {
                if(!File.Exists(inputFile))
                {
                    if(!args.Quiet)
                        Console.WriteLine($"    WARNING: Input file \"{inputFile}\" not found, skipping.");
                    continue;
                }

                if(!args.Quiet)
                {
                    Console.WriteLine("----------------------------------------------");
                    Console.WriteLine($"- {Path.GetFullPath(inputFile)}");
                    Console.WriteLine("----------------------------------------------");
                }

                using var sr = new StreamReader(inputFile);
                using var cr = new CsvHelper.CsvReader(sr,
                    new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        BadDataFound = a => 
                        {
                            var line = string.IsNullOrWhiteSpace(a.RawRecord) ? "" : $"# {a.RawRecord}";
                            sw.WriteLine(line);
                            if (!args.Quiet)
                                Console.WriteLine(line);
                        },
                    });
                
                while (cr.Read())
                {
                    var input = cr.GetRecord<InputRecord>();
                    var result = $"Amount owed: {input.AmountOwed:C}  Amount tendered: {input.AmountTendered} => " + (
                        input.AmountOwed < 0 ? "ERROR: Cannot owe negative money." :
                        input.AmountTendered < 0 ? "ERROR: Cannot tender negative money." :
                        input.AmountTendered < input.AmountOwed ? "ERROR: Need more money." :
                        input.AmountOwed == input.AmountTendered ? "No change owed." :
                        PrettyPrintChangeOwed(changeTabulator.TabulateChange(input.AmountOwed, input.AmountTendered)));
                    sw.WriteLine(result);
                    if (!args.Quiet)
                        Console.WriteLine(result);
                }
            }
        }

        private static string PrettyPrintChangeOwed(IEnumerable<KeyValuePair<Denomination, ulong>> changeOwed)
            => string.Join(", ",
                changeOwed
                    .OrderByDescending(kvp => kvp.Key)
                    .Select(kvp =>
                        kvp.Key == Denomination.Hundred ? $"{kvp.Value} Benjamin{(kvp.Value != 1 ? "s" : "")}" :
                        kvp.Key == Denomination.Fifty ? $"{kvp.Value} fift{(kvp.Value != 1 ? "ies" : "y")}" :
                        kvp.Key == Denomination.Twenty ? $"{kvp.Value} twent{(kvp.Value != 1 ? "ies" : "y")}" :
                        kvp.Key == Denomination.Ten ? $"{kvp.Value} ten{(kvp.Value != 1 ? "s" : "")}" :
                        kvp.Key == Denomination.Five ? $"{kvp.Value} five{(kvp.Value != 1 ? "s" : "")}" :
                        kvp.Key == Denomination.One ? $"{kvp.Value} one{(kvp.Value != 1 ? "s" : "")}" :
                        kvp.Key == Denomination.Quarter ? $"{kvp.Value} quarter{(kvp.Value != 1 ? "s" : "")}" :
                        kvp.Key == Denomination.Dime ? $"{kvp.Value} dime{(kvp.Value != 1 ? "s" : "")}" :
                        kvp.Key == Denomination.Nickel ? $"{kvp.Value} nickel{(kvp.Value != 1 ? "s" : "")}" :
                        kvp.Key == Denomination.Penny ? $"{kvp.Value} penn{(kvp.Value != 1 ? "ies" : "y")}" :
                        $"{kvp.Value} ????"));

        private class InputRecord
        {
            public decimal AmountOwed { get; set; }
            public decimal AmountTendered { get; set; }
        }
    }
}
