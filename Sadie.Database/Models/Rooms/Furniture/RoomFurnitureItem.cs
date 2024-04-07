using Sadie.Database.Models.Furniture;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Database.Models.Rooms.Furniture;

public class RoomFurnitureItem
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string OwnerUsername { get; set; }
    public FurnitureItem FurnitureItem { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public double PositionZ { get; set; }
    public string WallPosition { get; set; }
    public HDirection Direction { get; set; }
    public string LimitedData { get; set; }
    public string MetaData { get; set; }
    public DateTime CreatedAt { get; set; }
}