using Sadie.Database.Models.Furniture;

namespace Sadie.Database.Models.Players.Furniture;

public class PlayerFurnitureItem
{
    public int Id { get; init; }
    public int PlayerId { get; init; }
    public Player Player { get; init; }
    public FurnitureItem? FurnitureItem { get; set; }
    public ICollection<PlayerFurnitureItemPlacementData> PlacementData { get; init; } = [];
    public string? LimitedData { get; init; }
    public string? MetaData { get; set; }
    public DateTime CreatedAt { get; init; }
}