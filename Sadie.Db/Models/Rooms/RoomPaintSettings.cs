namespace Sadie.Db.Models.Rooms;

public class RoomPaintSettings
{
    public int Id { get; init; }
    public Room? Room { get; init; }
    public int RoomId { get; init; }
    public string FloorPaint { get; set; } = "";
    public string WallPaint { get; set; } = "";
    public string LandscapePaint { get; set; } = "";
}