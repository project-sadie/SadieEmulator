using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Players.Furniture;

namespace Sadie.Db.Configurations.Players.FurnitureItems;

public class PlayerFurnitureItemConfiguration : IEntityTypeConfiguration<PlayerFurnitureItem>
{
    public void Configure(EntityTypeBuilder<PlayerFurnitureItem> entity)
    {
        entity.ToTable("player_furniture_items");

        entity.Property(p => p.LimitedData).HasDefaultValue(string.Empty);
        entity.Property(p => p.MetaData).HasDefaultValue(string.Empty);

        entity.Navigation(x => x.FurnitureItem).AutoInclude();
        entity.Navigation(x => x.PlacementData).AutoInclude();
    }
}