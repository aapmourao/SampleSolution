namespace SharedKernel.Domain.Core;

public abstract class EntityRaw<TId>
{
    public TId Id { get; init; }

    protected EntityRaw(TId id)
    {
        Id = id;
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    protected EntityRaw() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


}

public abstract class Entity<TId> : EntityRaw<TId>, IAuditableEntity
{
    public EntityAuditFields AuditFields { get; init; } = new();

    public override bool Equals(object? other)
    {
        if (other is null || other.GetType() != GetType())
        {
            return false;
        }

        return ((Entity<TId>)other).Id!.Equals(Id);
    }

    public string GetEntityIdValue()
    {
        return Id!.ToString() ?? string.Empty;
    }

    public override int GetHashCode()
    {
        return Id!.GetHashCode();
    }

    protected Entity(TId id)
    {
        Id = id;
    }

    protected Entity() { }
}