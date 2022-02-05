namespace Sadie.Game.Rooms;

public class RoomModel
{
    public long Id { get; }
    public string Name { get; }
    public string HeightMap { get; }
    
    public RoomModel(long id, string name, string heightMap)
    {
        Id = id;
        Name = name;
        HeightMap = heightMap;
    }
}