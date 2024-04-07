namespace Sadie.Database.Models.Rooms;

public class RoomPaintSettings
{
    public int Id { get; set; }
    public Room Room { get; set; }
    public int RoomId { get; set; }
    public string FloorPaint { get; set; }
    public string WallPaint { get; set; }
    public string LandscapePaint { get; set; }
}