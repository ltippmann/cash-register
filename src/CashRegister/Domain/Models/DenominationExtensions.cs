using System.Collections.Generic;
using System.Linq;

namespace CashRegister.Domain.Models
{

    public static class DenominationExtensions
    {
        public static string PrettyPrint(this IEnumerable<KeyValuePair<Denomination, ulong>> changeOwed)
            => string.Join(", ",
                changeOwed
                    .OrderByDescending(kvp => kvp.Key)
                    .Select(kvp => PrettyPrint(kvp.Key, kvp.Value)));

        public static string PrettyPrint(this Denomination denomination, ulong amount)
            =>  denomination == Denomination.Hundred ? $"{amount} Benjamin{(amount != 1 ? "s" : "")}" :
                denomination == Denomination.Fifty ? $"{amount} fift{(amount != 1 ? "ies" : "y")}" :
                denomination == Denomination.Twenty ? $"{amount} twent{(amount != 1 ? "ies" : "y")}" :
                denomination == Denomination.Ten ? $"{amount} ten{(amount != 1 ? "s" : "")}" :
                denomination == Denomination.Five ? $"{amount} five{(amount != 1 ? "s" : "")}" :
                denomination == Denomination.One ? $"{amount} one{(amount != 1 ? "s" : "")}" :
                denomination == Denomination.Quarter ? $"{amount} quarter{(amount != 1 ? "s" : "")}" :
                denomination == Denomination.Dime ? $"{amount} dime{(amount != 1 ? "s" : "")}" :
                denomination == Denomination.Nickel ? $"{amount} nickel{(amount != 1 ? "s" : "")}" :
                denomination == Denomination.Penny ? $"{amount} penn{(amount != 1 ? "ies" : "y")}" :
                $"{amount} ????";
    }
}
