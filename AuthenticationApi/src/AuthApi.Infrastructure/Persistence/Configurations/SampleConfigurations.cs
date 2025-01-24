using AuthApi.Domain.SampleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static AuthApi.Domain.SampleAggregate.Sample;

namespace AuthApi.Infrastructure.Persistence.Configurations;

public class SampleConfigurations : IEntityTypeConfiguration<Sample>
{
    public void Configure(EntityTypeBuilder<Sample> builder)
    {
        builder.ToTable("Samples");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => SampleId.Create(value));

        builder.Property(u => u.Name);

        builder.Property(u => u.Description);

        // EntityIds
        builder.OwnsMany(e => e.EntityIds, entityIds =>
        {
            entityIds.ToTable("SampleEntityIds");

            entityIds.HasKey("Id");
            entityIds.Property(e => e.Value)
                .ValueGeneratedNever()
                .HasColumnName("SampleEntityId");
        });

        // Audit fields
        builder.OwnsOne(e => e.AuditFields, audit =>
        {
            audit.Property(u => u.CreatedAtUtc);
            audit.Property(u => u.CreatedBy);
            audit.Property(u => u.UpdatedAtUtc);
            audit.Property(u => u.UpdatedBy);
        });
    }
}
