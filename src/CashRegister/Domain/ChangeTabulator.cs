using System;
using System.Collections.Immutable;
using System.Linq;
using CashRegister.Domain.ChangeTabulationSrategies;
using CashRegister.Domain.Models;

namespace CashRegister.Domain
{
    public class ChangeTabulator
    {
        public ChangeTabulator(
            IChangeTabulationStrategy divisibleByThreeStrategy,
            IChangeTabulationStrategy notDivisibleByThreeStrategy)
        {
            _notDivisibleByThreeStrategy = notDivisibleByThreeStrategy;
            _divisibleByThreeStrategy = divisibleByThreeStrategy;
        }

        private readonly IChangeTabulationStrategy _notDivisibleByThreeStrategy;
        private readonly IChangeTabulationStrategy _divisibleByThreeStrategy;

        public virtual IImmutableDictionary<Denomination, ulong> TabulateChange(decimal amountDue, decimal amountTendered)
            =>  amountDue < 0 ? throw new ArgumentException("Amount due must be greater than zero.") :
                amountTendered < 0 ? throw new ArgumentException("Amount tendered must be greater than zero.") :
                amountTendered < amountDue ? throw new ArgumentException("Amount tendered must be greater than or equal amount due.") :
                TabulateChange((ulong)((amountTendered - amountDue) * 100));

        private IImmutableDictionary<Denomination, ulong> TabulateChange(ulong changeDueInCents)
            => DetermineTabulationStrategy(changeDueInCents)
                .Aggregate(changeDueInCents, _denominations);

        private IChangeTabulationStrategy DetermineTabulationStrategy(ulong changeDueInCents)
            => changeDueInCents % 3 != 0 ? _notDivisibleByThreeStrategy : _divisibleByThreeStrategy;

        private static readonly ImmutableArray<Denomination> _denominations =
            Enum.GetValues(typeof(Denomination))
            .Cast<Denomination>()
            .OrderByDescending(d => d)
            .ToImmutableArray();
    }
}
