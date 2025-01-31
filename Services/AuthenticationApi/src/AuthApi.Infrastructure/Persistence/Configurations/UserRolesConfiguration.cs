using Domain.UserRolesAggregate;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthApi.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(r => r.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedNever()
            .HasConversion(
                v => v.Value,
                v => UserId.Create(v));

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(100);

        // Configure many-to-many relationship  
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId);

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

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);
        builder.Property(u => u.Id)
            .ValueGeneratedNever()
            .HasConversion(
                v => v.Value,
                v => RoleId.Create(v));

        builder.Property(r => r.RoleName)
            .IsRequired()
            .HasMaxLength(100);

        // Configure many-to-many relationship  
        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId);

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

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.Property(u => u.UserId)
            .HasConversion(
                v => v.Value,
                v => UserId.Create(v));

        builder.Property(u => u.RoleId)
            .HasConversion(
                v => v.Value,
                v => RoleId.Create(v));

        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .HasPrincipalKey(u => u.Id);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .HasPrincipalKey(r => r.Id);
    }
}