using NUnit.Framework;
using NSubstitute;
using System;
using System.Collections.Immutable;
using CashRegister.Domain;
using CashRegister.Domain.ChangeTabulationSrategies;
using System.Collections.Generic;

namespace CashRegister.UnitTests
{
    public class ChangeTabulatorTests
    {
        private IImmutableDictionary<Denomination, ulong> divisibleByThreeResult;
        private IImmutableDictionary<Denomination, ulong> notDivisibleByThreeResult;
        private ChangeTabulator tabulator;

        [SetUp]
        public void Setup()
        {
            var divisibleByThreeStrategy = Substitute.For<IChangeTabulationStrategy>();
            divisibleByThreeResult = Substitute.For<IImmutableDictionary<Denomination, ulong>>();
            divisibleByThreeStrategy.Aggregate(Arg.Any<ulong>(), Arg.Any<IEnumerable<Denomination>>()).Returns(ci => divisibleByThreeResult);
            
            var notDivisibleByThreeStrategy = Substitute.For<IChangeTabulationStrategy>();
            notDivisibleByThreeResult = Substitute.For<IImmutableDictionary<Denomination, ulong>>();
            notDivisibleByThreeStrategy.Aggregate(Arg.Any<ulong>(), Arg.Any<IEnumerable<Denomination>>()).Returns(ci => notDivisibleByThreeResult);
            
            tabulator = Substitute.ForPartsOf<ChangeTabulator>(divisibleByThreeStrategy, notDivisibleByThreeStrategy);
        }

        [Test]
        public void GIVEN_an_amount_due_less_than_zero_WHEN_TabulateChange_is_called_THEN_an_exception_should_be_thrown()
        {
            Assert.Catch<ArgumentException>(() => tabulator.TabulateChange(-1, 10));
        }

        [Test]
        public void GIVEN_an_amount_tendered_less_than_zero_WHEN_TabulateChange_is_called_THEN_an_exception_should_be_thrown()
        {
            Assert.Catch<ArgumentException>(() => tabulator.TabulateChange(10, -1));
        }

        [Test]
        public void GIVEN_an_amount_tendered_less_than_the_amount_due_WHEN_TabulateChange_is_called_THEN_an_exception_should_be_thrown()
        {
            Assert.Catch<ArgumentException>(() => tabulator.TabulateChange(10, 1));
        }

        [Test]
        public void GIVEN_a_change_due_divisible_by_three_WHEN_TabulateChange_is_called_THEN_the_divisibleByThreeStrategy_should_be_used()
        {
            AssertIsDivisibleByThreeResult(tabulator.TabulateChange(2.0m, 5.0m)); // 2.00 % 0.03 = 0
            AssertIsDivisibleByThreeResult(tabulator.TabulateChange(2.0m, 52634.0m)); // 52632.00 % 0.03 = 0
        }


        [Test]
        public void GIVEN_a_change_due_not_divisible_by_three_WHEN_TabulateChange_is_called_THEN_the_notDivisibleByThreeStrategy_should_be_used()
        {
            AssertIsNotDivisibleByThreeResult(tabulator.TabulateChange(4.0m, 5.0m)); // 1.00 % 0.03 = 1
            AssertIsNotDivisibleByThreeResult(tabulator.TabulateChange(3.0m, 5.0m)); // 2.00 % 0.03 = 2
            AssertIsNotDivisibleByThreeResult(tabulator.TabulateChange(2.0m, 52635.0m)); // 52633.00 % 0.03 = 1
        }

        private void AssertIsDivisibleByThreeResult(IImmutableDictionary<Denomination, ulong> result)
        {
            Assert.AreNotSame(notDivisibleByThreeResult, result, $"Expected to receive {nameof(divisibleByThreeResult)} but received {nameof(notDivisibleByThreeResult)}.");
            Assert.AreSame(divisibleByThreeResult, result, $"Expected to receive {nameof(divisibleByThreeResult)} but received something else.");
        }

        private void AssertIsNotDivisibleByThreeResult(IImmutableDictionary<Denomination, ulong> result)
        {
            Assert.AreNotSame(divisibleByThreeResult, result, $"Expected to receive {nameof(notDivisibleByThreeResult)} but received {nameof(divisibleByThreeResult)}.");
            Assert.AreSame(notDivisibleByThreeResult, result, $"Expected to receive {nameof(notDivisibleByThreeResult)} but received something else.");
        }
   }
}