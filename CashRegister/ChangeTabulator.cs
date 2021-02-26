using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Immutable;

namespace CashRegister
{
    public class ChangeTabulator
    {
        public Dictionary<Denomination, int> MakeChange(int cents)
            => cents <= 0 ? throw new ArgumentException($"{nameof(cents)} must be a positive number greater than zero.") :
                _denominationArray.Aggregate(new Dictionary<Denomination, int>(),
                    (result, denom) => 
                    {
                        if()
                        return result;
                    });

        private static readonly ImmutableArray<Denomination> _denominationArray =
            (Enum.GetValues(typeof(Denomination)) as IEnumerable<Denomination>)
            .OrderByDescending(d => d)
            .ToImmutableArray();

        private 

    }
}
