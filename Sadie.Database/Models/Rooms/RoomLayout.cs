using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Database.Models.Rooms;

public class RoomLayout
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string HeightMap { get; set; }
    public int DoorX { get; set; }
    public int DoorY { get; set; }
    public double DoorZ { get; set; }
    public HDirection DoorDirection { get; set; }
}