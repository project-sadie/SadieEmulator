using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms;

public class RoomLayout : RoomLayoutData
{
    public long Id { get; }
    public string Name { get; }
    public string HeightMap { get; }
    public short[,] TileMap { get; }
    
    public RoomLayout(long id, string name, string heightMap, HPoint doorPoint, HDirection doorDirection) : base(heightMap, doorPoint, doorDirection)
    {
        Id = id;
        Name = name;
        HeightMap = heightMap;
        TileMap = new short[SizeY, SizeX];

        foreach (var tile in Tiles)
        {
            var point = tile.Point;
            TileMap[point.Y, point.X] = (short)(tile.State == RoomTileState.Open ? 1 : 0);
        }
    }
}