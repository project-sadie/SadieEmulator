using System.ComponentModel.DataAnnotations;

namespace Sadie.Database.Models.Players.Furniture;

public class PlayerFurnitureItemWiredData
{
    [Key] public int Id { get; init; }
    public int PlayerFurnitureItemId { get; init; }
    public required PlayerFurnitureItem PlayerFurnitureItem { get; init; }
    public required ICollection<PlayerFurnitureItem> SelectedItems { get; init; }
    public string? Message { get; init; }
    public int Delay { get; init; }
}