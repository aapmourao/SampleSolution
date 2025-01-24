namespace SharedKernel.Domain.Core;

/// <summary>
/// Reference for <Entity>Id classes
/// </summary>
public sealed class EntityId : AggregateRootId<Guid>
{
    public override Guid Value { get; protected set; }

    public EntityId(Guid value) { Value = value; }

    public static EntityId CreateUnique() => new EntityId(Guid.NewGuid());
    public static EntityId Create(Guid value) => new EntityId(value);

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}