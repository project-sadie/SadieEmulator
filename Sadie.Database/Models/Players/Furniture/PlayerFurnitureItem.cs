using Sadie.Database.Models.Furniture;

namespace Sadie.Database.Models.Players.Furniture;

public class PlayerFurnitureItem
{
    public int Id { get; init; }
    public int PlayerId { get; set; }
    public required Player Player { get; set; }
    public required FurnitureItem FurnitureItem { get; init; }
    public PlayerFurnitureItemPlacementData? PlacementData { get; set; }
    public PlayerFurnitureItemWiredData? WiredData { get; set; }
    public ICollection<PlayerFurnitureItemWiredData> SelectedBy { get; init; } = [];
    public required string LimitedData { get; init; }
    public required string MetaData { get; set; }
    public DateTime CreatedAt { get; init; }
}