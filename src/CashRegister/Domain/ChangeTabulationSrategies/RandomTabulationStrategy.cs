using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CashRegister.Domain.ChangeTabulationSrategies
{
    /// <summary>
    /// Tabulates money using a random distribution of denominations.
    /// </summary>
    public class RandomTabulationStrategy : IChangeTabulationStrategy
    {
        public IImmutableDictionary<Denomination, ulong> Aggregate(ulong changeDueInCents, IEnumerable<Denomination> descendingDenominations)
            => descendingDenominations
                .Cast<ushort>()
                .Aggregate(ImmutableDictionary.Create<Denomination, ulong>(),
                    (result, denom) =>
                    {
                        ulong denomValue;
                        // Subtract a random amount less than the remaining value of whatever denomination
                        //  unless we're on pennies, then throw the entire remainder in.
                        changeDueInCents -= denom * (denomValue = denom == 1 ? changeDueInCents : BigRandom(changeDueInCents / denom));
                        return denomValue > 0
                            ? result.Add((Denomination)denom, denomValue)
                            : result;
                    });

        private static readonly Random _rand = new Random();

        private static ulong BigRandom(ulong max)
        {
            var buf = new byte[8];
            _rand.NextBytes(buf);
            return BitConverter.ToUInt64(buf, 0) % max;
        }
    }
}
