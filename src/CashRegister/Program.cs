using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CashRegister.Domain;
using CashRegister.Domain.Models;
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
            [Option('o', "output", HelpText = "File to write processing output to.")]
            public string OutputFile { get; set; }
            [Option('q', "quiet")]
            public bool Quiet { get; set; }
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

            using var sw = string.IsNullOrEmpty(args.OutputFile) ? null : new StreamWriter(args.OutputFile);
            
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
                        //BadDataFound = a => 
                        //{
                        //    var line = string.IsNullOrWhiteSpace(a.RawRecord) ? "" : $"# {a.RawRecord}";
                        //    sw?.WriteLine(line);
                        //    if (!args.Quiet)
                        //        Console.WriteLine(line);
                            //},
                            HasHeaderRecord = false,
                        HeaderValidated = null,
                        IgnoreBlankLines = false,
                        ShouldSkipRecord = a =>
                        {
                            if(a.Record.Length != 2 || !decimal.TryParse(a.Record[0], out _) || !decimal.TryParse(a.Record[1], out _))
                            {
                                var line = string.Join(",", a.Record);
                                sw?.WriteLine(line);
                                if (!args.Quiet)
                                    Console.WriteLine(line);
                                return true;
                            }
                            return false;
                        }
                    });
                
                while (cr.Read())
                {
                    var input = cr.GetRecord<InputRecord>();
                    var result = $"Amount owed: {input.AmountOwed:C}  Amount tendered: {input.AmountTendered:C}" + (
                        input.AmountOwed < 0 ? "ERROR: Cannot owe negative money." :
                        input.AmountTendered < 0 ? "ERROR: Cannot tender negative money." :
                        input.AmountTendered < input.AmountOwed ? "ERROR: Need more money." :
                        input.AmountOwed == input.AmountTendered ? "No change owed." :
                        ($"  Change due: {input.AmountTendered - input.AmountOwed:C}{IsDivisibleByThree(input.AmountOwed, input.AmountTendered)} => "
                            + changeTabulator.TabulateChange(input.AmountOwed, input.AmountTendered)
                                .PrettyPrint()));
                    sw?.WriteLine(result);
                    if (!args.Quiet)
                        Console.WriteLine(result);
                }
            }
        }

        private static string IsDivisibleByThree(decimal owed, decimal tendered)
            => ((ulong)(tendered * 100) - (ulong)(owed * 100)) % 3 == 0 ? "*" : "";

        private class InputRecord
        {
            public decimal AmountOwed { get; set; }
            public decimal AmountTendered { get; set; }
        }
    }
}
