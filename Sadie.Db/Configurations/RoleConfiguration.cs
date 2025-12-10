using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models;

namespace Sadie.Db.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> entity)
    {
        entity.ToTable("roles");

        entity.Navigation(x => x.Permissions).AutoInclude();

        entity.HasMany(r => r.Permissions)
            .WithMany(p => p.Roles)
            .UsingEntity("roles_permissions",
                l => l.HasOne(typeof(Permission)).WithMany().HasForeignKey("permission_id").HasPrincipalKey(nameof(Permission.Id)),
                r => r.HasOne(typeof(Role)).WithMany().HasForeignKey("role_id").HasPrincipalKey(nameof(Role.Id)),
                j => j.HasKey("permission_id", "role_id"));
    }
}