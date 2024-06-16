using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Database.Models.Furniture;

namespace Sadie.Database.Models.Players.Furniture;

public class PlayerFurnitureItem
{
    public int Id { get; init; }
    public int PlayerId { get; init; }
    public Player? Player { get; init; }
    public FurnitureItem? FurnitureItem { get; set; }
    public PlayerFurnitureItemPlacementData? PlacementData { get; set; }
    public string? LimitedData { get; init; }
    public string? MetaData { get; set; }
    public DateTime CreatedAt { get; init; }
}