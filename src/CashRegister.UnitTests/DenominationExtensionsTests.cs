using CashRegister.Domain.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace CashRegister.UnitTests
{
    [TestFixture]
    public class DenominationExtensionsTests
    {
        [Test]
        public void GIVEN_an_amount_of_1_WHEN_a_denomination_is_pretty_print_THEN_the_singular_form_should_be_used()
        {
            Assert.AreEqual("1 Benjamin", Denomination.Hundred.PrettyPrint(1));
            Assert.AreEqual("1 fifty", Denomination.Fifty.PrettyPrint(1));
            Assert.AreEqual("1 twenty", Denomination.Twenty.PrettyPrint(1));
            Assert.AreEqual("1 ten", Denomination.Ten.PrettyPrint(1));
            Assert.AreEqual("1 five", Denomination.Five.PrettyPrint(1));
            Assert.AreEqual("1 one", Denomination.One.PrettyPrint(1));
            Assert.AreEqual("1 quarter", Denomination.Quarter.PrettyPrint(1));
            Assert.AreEqual("1 dime", Denomination.Dime.PrettyPrint(1));
            Assert.AreEqual("1 nickel", Denomination.Nickel.PrettyPrint(1));
            Assert.AreEqual("1 penny", Denomination.Penny.PrettyPrint(1));
        }

        [Test]
        public void GIVEN_an_amount_of_2_WHEN_a_denomination_is_pretty_print_THEN_the_plural_form_should_be_used()
        {
            Assert.AreEqual("2 Benjamins", Denomination.Hundred.PrettyPrint(2));
            Assert.AreEqual("2 fifties", Denomination.Fifty.PrettyPrint(2));
            Assert.AreEqual("2 twenties", Denomination.Twenty.PrettyPrint(2));
            Assert.AreEqual("2 tens", Denomination.Ten.PrettyPrint(2));
            Assert.AreEqual("2 fives", Denomination.Five.PrettyPrint(2));
            Assert.AreEqual("2 ones", Denomination.One.PrettyPrint(2));
            Assert.AreEqual("2 quarters", Denomination.Quarter.PrettyPrint(2));
            Assert.AreEqual("2 dimes", Denomination.Dime.PrettyPrint(2));
            Assert.AreEqual("2 nickels", Denomination.Nickel.PrettyPrint(2));
            Assert.AreEqual("2 pennies", Denomination.Penny.PrettyPrint(2));
        }

        [Test]
        public void GIVEN_a_dictionary_of_Denominations_and_amounts_WHEN_the_dictionary_is_pretty_print_THEN_the_correct_result_should_be_returned()
        {
            Assert.AreEqual("10 Benjamins, 9 fifties, 8 twenties, 6 fives, 5 ones, 3 dimes, 2 nickels, 1 penny",
                new Dictionary<Denomination, ulong>
                {
                    { Denomination.Penny, 1 },
                    { Denomination.Twenty, 8 },
                    { Denomination.Five, 6 },
                    { Denomination.Fifty, 9 },
                    { Denomination.Nickel, 2 },
                    { Denomination.One, 5 },
                    { Denomination.Hundred, 10 },
                    { Denomination.Dime, 3 },
                }.PrettyPrint());
        }
    }
}
