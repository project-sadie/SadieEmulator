using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Players.Furniture;

public class PlayerFurnitureItemWiredData
{
    [Key] public int Id { get; init; }
    public int PlacementDataId { get; init; }
    public required PlayerFurnitureItemPlacementData PlacementData { get; init; }
    public required ICollection<PlayerFurnitureItem> FurnitureItems { get; init; }
    public string? Message { get; init; }
    public int Delay { get; init; }
}