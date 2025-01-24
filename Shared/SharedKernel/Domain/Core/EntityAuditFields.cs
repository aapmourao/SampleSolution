namespace SharedKernel.Domain.Core;

public class EntityAuditFields
{
    public string UpdatedBy { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; } = null!;

    public EntityAuditFields()
    {
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Update(string updatedBy)
    {
        UpdatedBy = updatedBy;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
