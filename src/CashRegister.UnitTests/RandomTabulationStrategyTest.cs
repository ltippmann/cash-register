using CashRegister.Domain;
using CashRegister.Domain.ChangeTabulationSrategies;
using NUnit.Framework;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace CashRegister.UnitTests
{
    [TestFixture]
    public class RandomTabulationStrategyTest
    {
        private static readonly ImmutableArray<Denomination> _denominations =
            Enum.GetValues(typeof(Denomination))
            .Cast<Denomination>()
            .OrderByDescending(d => d)
            .ToImmutableArray();

        [Test]
        public void GIVEN_a_valid_changeDueInCents_WHEN_Aggregate_is_called_multiple_times_THEN_different_results_should_usually_be_returned()
        {
            // Since the RandomTabulationStrategy produces a non-deterministic result it's impossible to write
            //  a test we can be fully confident in.  This test may produce false negatives.  By giving the 
            //  random number generator enough room to jiggle around (by picking large quantity of change to
            //  tabulate) the false negatives should be very infrequent.

            var result1 = new RandomTabulationStrategy().Aggregate(123456789, _denominations);
            Assert.AreEqual(123456789, result1.Sum(kvp => (long)((ushort)kvp.Key * kvp.Value)), "Change did not produce the correct total.");

            var result2 = new RandomTabulationStrategy().Aggregate(123456789, _denominations);
            Assert.AreEqual(123456789, result2.Sum(kvp => (long)((ushort)kvp.Key * kvp.Value)), "Change did not produce the correct total.");

            var result3 = new RandomTabulationStrategy().Aggregate(123456789, _denominations);
            Assert.AreEqual(123456789, result3.Sum(kvp => (long)((ushort)kvp.Key * kvp.Value)), "Change did not produce the correct total.");

            CollectionAssert.AreNotEquivalent(result1, result2, "WARNING: This may be a false negative.");
            CollectionAssert.AreNotEquivalent(result1, result3, "WARNING: This may be a false negative.");
            CollectionAssert.AreNotEquivalent(result2, result3, "WARNING: This may be a false negative.");
        }
    }
}
