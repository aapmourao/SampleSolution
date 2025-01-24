namespace SharedKernel.Domain.Core;

public interface IAuditableEntity
{
    EntityAuditFields AuditFields { get; }
}
