using SharedKernel.Domain.Common;

namespace SharedKernel.Domain.Core;

public abstract class AggregateRootId<TId> : ValueObject
{
    public abstract TId Value { get; protected set; }
}