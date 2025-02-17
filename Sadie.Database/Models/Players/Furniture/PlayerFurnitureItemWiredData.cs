using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Players.Furniture;

public class PlayerFurnitureItemWiredData
{
    [Key] public int Id { get; init; }
    public required int PlayerFurnitureItemPlacementDataId { get; init; }
    public required PlayerFurnitureItemPlacementData PlacementData { get; init; }
    public required string? Message { get; init; }
    public int Delay { get; init; }
    public required List<PlayerFurnitureItemWiredParameter> PlayerFurnitureItemWiredParameters { get; init; }
    public required List<PlayerFurnitureItemWiredItem> PlayerFurnitureItemWiredItems { get; init; }
}