namespace Sadie.Database.Models.Rooms;

public class RoomPaintSettings
{
    public int Id { get; init; }
    public Room Room { get; init; }
    public int RoomId { get; init; }
    public string FloorPaint { get; set; } = "0.0";
    public string WallPaint { get; set; } = "0.0";
    public string LandscapePaint { get; set; } = "0.0";
}