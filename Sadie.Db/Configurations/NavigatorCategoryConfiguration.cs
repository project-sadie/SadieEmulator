using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Navigator;

namespace Sadie.Db.Configurations;

public class NavigatorCategoryConfiguration : IEntityTypeConfiguration<NavigatorCategory>
{
    public void Configure(EntityTypeBuilder<NavigatorCategory> entity)
    {
        entity.HasOne(e => e.Tab)
            .WithMany(t => t.Categories)
            .HasForeignKey(e => e.TabId);
    }
}
