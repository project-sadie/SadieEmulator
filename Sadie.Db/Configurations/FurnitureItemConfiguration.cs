using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Shared.Helpers;
using Sadie.Db.Models.Furniture;

namespace Sadie.Db.Configurations;

public class FurnitureItemConfiguration : IEntityTypeConfiguration<FurnitureItem>
{
    public void Configure(EntityTypeBuilder<FurnitureItem> entity)
    {
        entity.Property(e => e.Type)
            .HasConversion(
                v => EnumHelpers.GetEnumDescription(v),
                v => EnumHelpers.GetEnumValueFromDescription<FurnitureItemType>(v));

        entity.Navigation(x => x.HandItems).AutoInclude();

        entity.HasMany(f => f.HandItems)
            .WithMany(h => h.FurnitureItems);
    }
}