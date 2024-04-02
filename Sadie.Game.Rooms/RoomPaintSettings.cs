namespace Sadie.Game.Rooms;

public class RoomPaintSettings(
    string floorPaint,
    string wallPaint,
    string landscapePaint)
{
    public string FloorPaint { get; set; } = floorPaint;
    public string WallPaint { get; set; } = wallPaint;
    public string LandscapePaint { get; set; } = landscapePaint;
}