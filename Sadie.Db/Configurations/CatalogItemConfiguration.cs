using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Catalog.Items;

namespace Sadie.Db.Configurations;

public class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> entity)
    {
        entity.HasMany(c => c.FurnitureItems)
            .WithMany(f => f.CatalogItems);
    }
}

