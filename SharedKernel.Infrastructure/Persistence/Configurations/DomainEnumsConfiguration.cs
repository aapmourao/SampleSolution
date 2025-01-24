using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.Common;

namespace SharedKernel.Infrastructure.EventualConsistency;
internal class DomainEnumsConfiguration : IEntityTypeConfiguration<DomainEnum>
{
    public void Configure(EntityTypeBuilder<DomainEnum> builder)
    {
        builder.ToTable("_DomainEnums");
        builder.HasKey(x => new { x.Id });

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.EnumName);
        builder.Property(e => e.Name);

        builder.Property(e => e.Value);

        builder.Property(e => e.Metadata);
    }
}