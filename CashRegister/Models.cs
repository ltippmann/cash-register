using System;
using System.Collections.Generic;
using System.Text;

namespace CashRegister
{
    public enum Denomination: ushort
    {
        Penny = 1,
        Nickel = 5,
        Dime = 10,
        Quarter = 25,
        One = 100,
        Five = 500,
        Ten = 1000,
        Twenty = 2000,
        Fifty = 5000,
        Hundred = 10000,
    }
}
