using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

namespace CashRegister
{
    public class ChangeTabulator
    {
        public virtual ImmutableDictionary<Denomination, ushort> MakeChange(decimal amountDue, decimal amountTendered)
        {
            var changeDueInCents = amountTendered < amountDue ? throw new ArgumentException("Amount tendered is less than amount due.") :
                (ushort)((amountTendered - amountDue) * 100);

            return _denominationArray.Cast<ushort>().Aggregate(
                ImmutableDictionary.Create<Denomination, ushort>(),
                changeDueInCents % 3 != 0 ? BigEndianAggregator(changeDueInCents) : RandomAggregator(changeDueInCents));
        }

        private static readonly ImmutableArray<Denomination> _denominationArray =
            (Enum.GetValues(typeof(Denomination)) as IEnumerable<Denomination>)
            .OrderByDescending(d => d)
            .ToImmutableArray();

        internal virtual Func<ImmutableDictionary<Denomination, ushort>, ushort, ImmutableDictionary<Denomination, ushort>>
            BigEndianAggregator(ushort cents) => (result, denom) =>
            {
                ushort denomValue;
                // Subtract as many of whatever denomination as we can
                cents -= (ushort)(denom * (denomValue = (ushort)(cents / denom)));
                return denomValue > 0
                    ? result.Add((Denomination)denom, denomValue)
                    : result;
            };

        internal static readonly Random _rand = new Random();
        public virtual Func<ImmutableDictionary<Denomination, ushort>, ushort, ImmutableDictionary<Denomination, ushort>>
            RandomAggregator(ushort cents) => (result, denom) =>
            {
                ushort denomValue;
                // Subtract a random amount less than the remaining value of whatever denomination
                //  unless we're on pennies, then throw the entire remainder in.
                cents -= (ushort)(denom * (denomValue = denom == 1 ? cents : (ushort)_rand.Next(cents / denom)));
                return denomValue > 0
                    ? result.Add((Denomination)denom, denomValue)
                    : result;
            };
    }
}
