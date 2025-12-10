using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Players.Furniture;

namespace Sadie.Db.Configurations.Players;

public class PlayerFurnitureItemPlacementDataConfiguration : IEntityTypeConfiguration<PlayerFurnitureItemPlacementData>
{
    public void Configure(EntityTypeBuilder<PlayerFurnitureItemPlacementData> entity)
    {
        entity.ToTable("player_furniture_item_placement_data");

        entity.Navigation(x => x.PlayerFurnitureItem).AutoInclude();
        entity.Navigation(x => x.WiredData).AutoInclude();
    }
}