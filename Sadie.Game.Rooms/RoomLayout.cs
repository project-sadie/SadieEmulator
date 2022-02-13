using Sadie.Shared;

namespace Sadie.Game.Rooms;

public class RoomLayout : RoomLayoutData
{
    public long Id { get; }
    public string Name { get; }
    public string HeightMap { get; }
    
    public HPoint DoorPoint { get; }
    
    public RoomLayout(long id, string name, string heightMap, HPoint doorPoint) : base(heightMap)
    {
        Id = id;
        Name = name;
        HeightMap = heightMap;
        DoorPoint = doorPoint;
    }
}