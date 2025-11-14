using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Db.Models.Furniture;
using Sadie.Db.Models.Rooms;

namespace Sadie.Db.Models.Players.Furniture;

public class PlayerFurnitureItemPlacementData
{
    [Key] public int Id { get; init; }
    public int PlayerFurnitureItemId { get; init; }
    public required PlayerFurnitureItem PlayerFurnitureItem { get; init; }
    public int RoomId { get; init; }
    public Room? Room { get; init; }z
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public double PositionZ { get; set; }
    public string? WallPosition { get; set; }
    public HDirection Direction { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public PlayerFurnitureItemWiredData? WiredData { get; set; }
    
    public ICollection<PlayerFurnitureItemWiredData> SelectedBy { get; init; } = [];

    [NotMapped] public FurnitureItem FurnitureItem => PlayerFurnitureItem.FurnitureItem;
}