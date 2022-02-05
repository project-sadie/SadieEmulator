namespace Sadie.Game.Rooms;

public class RoomLayout
{
    public long Id { get; }
    public string Name { get; }
    public string HeightMap { get; }
    
    public RoomLayout(long id, string name, string heightMap)
    {
        Id = id;
        Name = name;
        HeightMap = heightMap;
    }
}