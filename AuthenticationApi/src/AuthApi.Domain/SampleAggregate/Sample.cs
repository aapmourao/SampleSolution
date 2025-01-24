using AuthApi.Domain.SampleAggregate.Events;
using ErrorOr;
using SharedKernel.Domain.Core;
using static AuthApi.Domain.SampleAggregate.Sample;

namespace AuthApi.Domain.SampleAggregate;

public class Sample : AggregateRoot<SampleId, Guid>
{
    public Sample(string name, string description) : base(new SampleId(Guid.NewGuid()))
    {
        Name = name;
        Description = description;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }

    public List<EntityId> EntityIds { get; private set; } = new();

    public void SampleAdded()
    {
        _domainEvents.Add(new SampleCreatedEvent(this.Id.Value));
    }

    public ErrorOr<bool> AddEntityId(EntityId entityId)
    {
        if (entityId == null) return Error.Failure("EntityId is required.");
        if (EntityIds.Any(e => e.Value == entityId.Value)) return Error.Conflict("EntityId already exists.");
        EntityIds.Add(entityId);

        return true;
    }

    public void Update(string? name, string? description)
    {
        Name = name ?? Name;
        Description = description ?? Description;
    }

    public class SampleId : AggregateRootId<Guid>
    {
        public override Guid Value { get; protected set; }

        public SampleId(Guid value) { Value = value; }
        public static SampleId CreateUnique() => new(Guid.NewGuid());
        public static SampleId Create(Guid value) => new(value);

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value.ToString();
    }
}
