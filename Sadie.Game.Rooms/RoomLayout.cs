using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms;

public class RoomLayout(
    int id,
    string name,
    string heightMap,
    HPoint doorPoint,
    HDirection doorDirection)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public string HeightMap { get; } = heightMap;
    public int DoorX { get; set; } = doorPoint.X;
    public int DoorY { get; set; } = doorPoint.Y;
    public double DoorZ { get; set; } = doorPoint.Z;
    public HDirection DoorDirection { get; } = doorDirection;
}