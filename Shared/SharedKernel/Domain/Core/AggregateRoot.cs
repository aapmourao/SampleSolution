using SharedKernel.Domain.Common;

namespace SharedKernel.Domain.Core;

public abstract class AggregateRoot<TId, TIdType> : Entity<TId>
    where TId : AggregateRootId<TIdType>
{
    public new AggregateRootId<TIdType> Id { get; protected set; }
    protected AggregateRoot(TId id)
    {
        Id = id;
    }

    protected readonly List<IDomainEvent> _domainEvents = new();

    public List<IDomainEvent> PopDomainEvents()
    {
        var copy = _domainEvents.ToList();
        _domainEvents.Clear();

        return copy;
    }

#pragma warning disable CS8618
    protected AggregateRoot() { }
#pragma warning restore CS8618

}