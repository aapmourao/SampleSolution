using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Infrastructure.Persistence;

namespace SharedKernel.Infrastructure.Common.Configurations;
internal class DbVersionsConfiguration : IEntityTypeConfiguration<DbVersion>
{
    public void Configure(EntityTypeBuilder<DbVersion> builder)
    {
        builder.ToTable("__DbVersions");
        builder.HasKey(x => new { x.Version });
        builder.Property(x => x.Script).IsRequired();
    }
}