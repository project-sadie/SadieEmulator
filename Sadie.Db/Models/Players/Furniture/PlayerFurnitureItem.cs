using Sadie.Db.Models.Furniture;

namespace Sadie.Db.Models.Players.Furniture;

public class PlayerFurnitureItem
{
    public int Id { get; init; }
    public long PlayerId { get; set; }

    public required Player Player { get; set; }
    public required FurnitureItem FurnitureItem { get; init; }

    public PlayerFurnitureItemPlacementData? PlacementData { get; set; }

    public required string LimitedData { get; init; }
    public required string MetaData { get; set; }

    public DateTimeOffset CreatedAt { get; init; }
}