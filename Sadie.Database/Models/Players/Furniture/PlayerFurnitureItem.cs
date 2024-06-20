using Sadie.Database.Models.Furniture;

namespace Sadie.Database.Models.Players.Furniture;

public class PlayerFurnitureItem
{
    public int Id { get; init; }
    public int PlayerId { get; set; }
    public Player? Player { get; set; }
    public FurnitureItem? FurnitureItem { get; set; }
    public PlayerFurnitureItemPlacementData? PlacementData { get; set; }
    public required string LimitedData { get; init; }
    public required string MetaData { get; set; }
    public DateTime CreatedAt { get; init; }
}