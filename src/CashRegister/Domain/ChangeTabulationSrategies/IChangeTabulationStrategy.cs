using CashRegister.Domain.Models;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CashRegister.Domain.ChangeTabulationSrategies
{
    public interface IChangeTabulationStrategy
    {
        IImmutableDictionary<Denomination, ulong> Aggregate(ulong changeDueInCents, IEnumerable<Denomination> descendingDenominations);
    }
}
