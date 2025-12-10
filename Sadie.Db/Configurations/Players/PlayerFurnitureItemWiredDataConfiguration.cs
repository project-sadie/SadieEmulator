using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sadie.Db.Models.Players.Furniture;

namespace Sadie.Db.Configurations.Players;

public class PlayerFurnitureItemWiredDataConfiguration : IEntityTypeConfiguration<PlayerFurnitureItemWiredData>
{
    public void Configure(EntityTypeBuilder<PlayerFurnitureItemWiredData> entity)
    {
        entity.HasOne(w => w.PlacementData)
            .WithOne(p => p.WiredData)
            .HasForeignKey<PlayerFurnitureItemWiredData>(w => w.PlayerFurnitureItemPlacementDataId);

        entity.HasMany(w => w.SelectedItems)
            .WithMany(i => i.SelectedBy)
            .UsingEntity("player_furniture_item_wired_data_items",
                l => l.HasOne(typeof(PlayerFurnitureItemPlacementData)).WithMany().HasForeignKey("player_furniture_item_placement_data_id"),
                r => r.HasOne(typeof(PlayerFurnitureItemWiredData)).WithMany().HasForeignKey("player_furniture_item_wired_data_id"),
                j => j.HasKey("player_furniture_item_placement_data_id", "player_furniture_item_wired_data_id"));
    }
}