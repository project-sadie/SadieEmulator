using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Database.Models.Furniture;
using Sadie.Database.Models.Rooms;
using Sadie.Enums.Unsorted;

namespace Sadie.Database.Models.Players.Furniture;

public class PlayerFurnitureItemPlacementData
{
    [Key] public int Id { get; init; }
    public int PlayerFurnitureItemId { get; init; }
    public required PlayerFurnitureItem PlayerFurnitureItem { get; set; }
    public int RoomId { get; init; }
    public Room? Room { get; init; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public double PositionZ { get; set; }
    public string? WallPosition { get; set; }
    public HDirection Direction { get; set; }
    public DateTime CreatedAt { get; init; }
    public PlayerFurnitureItemWiredData? WiredData { get; set; }

    [NotMapped] public FurnitureItem FurnitureItem => PlayerFurnitureItem.FurnitureItem;
}