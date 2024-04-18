namespace Sadie.Database.Models.Rooms;

public class RoomPaintSettings
{
    public int Id { get; set; }
    public Room Room { get; set; }
    public int RoomId { get; set; }
    public string FloorPaint { get; set; } = "0.0";
    public string WallPaint { get; set; } = "0.0";
    public string LandscapePaint { get; set; } = "0.0";
}