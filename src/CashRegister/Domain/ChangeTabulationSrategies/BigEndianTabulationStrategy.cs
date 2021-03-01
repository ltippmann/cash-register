using CashRegister.Domain.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CashRegister.Domain.ChangeTabulationSrategies
{
    /// <summary>
    /// Tabulates money preferring the largest denominations.
    /// </summary>
    public class BigEndianTabulationStrategy : IChangeTabulationStrategy
    {
        public IImmutableDictionary<Denomination, ulong> Aggregate(ulong changeDueInCents, IEnumerable<Denomination> descendingDenominations)
            => descendingDenominations
                .Cast<ushort>()
                .Aggregate(ImmutableDictionary.Create<Denomination, ulong>(),
                    (result, denom) =>
                    {
                        ulong denomValue;
                        // Subtract as many of whatever denomination as we can
                        changeDueInCents -= denom * (denomValue = changeDueInCents / denom);
                        return denomValue > 0
                            ? result.Add((Denomination)denom, denomValue)
                            : result;
                    });
    }
}
