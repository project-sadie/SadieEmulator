using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Database.Models.Furniture;
using Sadie.Database.Models.Rooms;
using Sadie.Enums.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Database.Models.Players.Furniture;

public class PlayerFurnitureItemPlacementData
{
    [Key] public int Id { get; init; }
    public int PlayerFurnitureItemId { get; init; }
    public PlayerFurnitureItem? PlayerFurnitureItem { get; init; }
    public int RoomId { get; init; }
    public Room? Room { get; init; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public double PositionZ { get; set; }
    public string? WallPosition { get; set; }
    public HDirection Direction { get; set; }
    public DateTime CreatedAt { get; init; }

    [NotMapped] public FurnitureItem? FurnitureItem
    {
        get => PlayerFurnitureItem?.FurnitureItem;
        init
        {
            if (PlayerFurnitureItem != null)
            {
                PlayerFurnitureItem.FurnitureItem = value;
            }
        }
    }
}