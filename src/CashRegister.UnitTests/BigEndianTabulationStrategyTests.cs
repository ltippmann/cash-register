using CashRegister.Domain;
using CashRegister.Domain.ChangeTabulationSrategies;
using CashRegister.Domain.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CashRegister.UnitTests
{
    [TestFixture]
    public class BigEndianTabulationStrategyTests
    {
        private static readonly ImmutableArray<Denomination> _denominations =
            Enum.GetValues(typeof(Denomination))
            .Cast<Denomination>()
            .OrderByDescending(d => d)
            .ToImmutableArray();

        [Test]
        public void GIVEN_a_valid_changeDueInCents_WHEN_Aggregate_is_called_THEN_the_correct_result_should_be_returned()
        {
            CollectionAssert.AreEquivalent(
                new Dictionary<Denomination, ulong>
                {
                    { Denomination.Penny, 1 },
                    { Denomination.Nickel, 1 },
                    { Denomination.Dime, 1 },
                    { Denomination.Quarter, 1 },
                    { Denomination.One, 1 },
                    { Denomination.Five, 1 },
                    { Denomination.Ten, 1 },
                    { Denomination.Twenty, 1 },
                    { Denomination.Fifty, 1 },
                    { Denomination.Hundred, 1 },
                },
                new BigEndianTabulationStrategy().Aggregate(18641, _denominations));

            CollectionAssert.AreEquivalent(
                new Dictionary<Denomination, ulong>
                {
                    { Denomination.Penny, 1 },
                    { Denomination.Nickel, 1 },
                    { Denomination.Dime, 1 },
                    { Denomination.Quarter, 1 },
                    { Denomination.One, 1 },
                    { Denomination.Five, 1 },
                    { Denomination.Ten, 1 },
                    { Denomination.Twenty, 1 },
                    { Denomination.Fifty, 3 },
                },
                new BigEndianTabulationStrategy().Aggregate(18641, _denominations.Skip(1)));

            CollectionAssert.AreEquivalent(
                new Dictionary<Denomination, ulong>
                {
                    { Denomination.Penny, 1 },
                    { Denomination.Nickel, 1 },
                    { Denomination.Dime, 1 },
                    { Denomination.Quarter, 1 },
                    { Denomination.One, 1 },
                    { Denomination.Five, 1 },
                    { Denomination.Twenty, 9 },
                },
                new BigEndianTabulationStrategy().Aggregate(18641, _denominations.Skip(2)));

            CollectionAssert.AreEquivalent(
                new Dictionary<Denomination, ulong>
                {
                    { Denomination.Penny, 1 },
                    { Denomination.Nickel, 1 },
                    { Denomination.Dime, 1 },
                    { Denomination.Quarter, 1 },
                    { Denomination.One, 1 },
                    { Denomination.Five, 1 },
                    { Denomination.Ten, 18 },
                },
                new BigEndianTabulationStrategy().Aggregate(18641, _denominations.Skip(3)));

            CollectionAssert.AreEquivalent(
                new Dictionary<Denomination, ulong>
                {
                    { Denomination.Penny, 1 },
                    { Denomination.Nickel, 1 },
                    { Denomination.Dime, 1 },
                    { Denomination.Quarter, 1 },
                    { Denomination.One, 1 },
                    { Denomination.Five, 37 },
                },
                new BigEndianTabulationStrategy().Aggregate(18641, _denominations.Skip(4)));

            CollectionAssert.AreEquivalent(
                new Dictionary<Denomination, ulong>
                {
                    { Denomination.Penny, 1 },
                    { Denomination.Nickel, 1 },
                    { Denomination.Dime, 1 },
                    { Denomination.Quarter, 1 },
                    { Denomination.One, 186 },
                },
                new BigEndianTabulationStrategy().Aggregate(18641, _denominations.Skip(5)));

            CollectionAssert.AreEquivalent(
                new Dictionary<Denomination, ulong>
                {
                    { Denomination.Penny, 1 },
                    { Denomination.Nickel, 1 },
                    { Denomination.Dime, 1 },
                    { Denomination.Quarter, 745 },
                },
                new BigEndianTabulationStrategy().Aggregate(18641, _denominations.Skip(6)));

            CollectionAssert.AreEquivalent(
                new Dictionary<Denomination, ulong>
                {
                    { Denomination.Penny, 1 },
                    { Denomination.Dime, 1864 },
                },
                new BigEndianTabulationStrategy().Aggregate(18641, _denominations.Skip(7)));

            CollectionAssert.AreEquivalent(
                new Dictionary<Denomination, ulong>
                {
                    { Denomination.Penny, 1 },
                    { Denomination.Nickel, 3728 },
                },
                new BigEndianTabulationStrategy().Aggregate(18641, _denominations.Skip(8)));

            CollectionAssert.AreEquivalent(
                new Dictionary<Denomination, ulong>
                {
                    { Denomination.Penny, 18641 },
                },
                new BigEndianTabulationStrategy().Aggregate(18641, _denominations.Skip(9)));
        }
    }
}
