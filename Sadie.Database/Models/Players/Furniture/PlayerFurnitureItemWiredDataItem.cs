using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Players.Furniture;

public class PlayerFurnitureItemWiredDataItem
{
    [Key] public int Id { get; init; }
    public int PlayerFurnitureItemPlacementDataId { get; init; }
    public PlayerFurnitureItemPlacementData PlayerFurnitureItemPlacementData { get; set; }
    public int PlayerFurnitureItemWiredDataId { get; init; }
}