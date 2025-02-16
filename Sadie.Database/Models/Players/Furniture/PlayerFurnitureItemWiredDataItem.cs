using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Players.Furniture;

public class PlayerFurnitureItemWiredDataItem
{
    [Key] public int Id { get; init; }
    public int PlayerFurnitureItemPlacementDataId { get; init; }
    public int PlayerFurnitureItemWiredDataId { get; init; }
}