using Sadie.Database.Models.Furniture;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Database.Models.Rooms.Furniture;

public class RoomFurnitureItem
{
    public int Id { get; init; }
    public int RoomId { get; init; }
    public int OwnerId { get; init; }
    public string? OwnerUsername { get; init; }
    public FurnitureItem? FurnitureItem { get; init; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public double PositionZ { get; set; }
    public string? WallPosition { get; set; }
    public HDirection Direction { get; set; }
    public string? LimitedData { get; init; }
    public string? MetaData { get; set; }
    public DateTime CreatedAt { get; init; }
}