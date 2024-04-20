namespace Sadie.Game.Rooms.Tiles;

public class RoomTileMap
{
    private List<string> HeightmapRows { get; }
    
    public int SizeX { get; }
    public int SizeY { get; }
    public int Size { get; }
    public short[,] Map { get; private set; }

    public RoomTileMap(string heightmap, short[,] map)
    {
        HeightmapRows = heightmap.Split("\n").ToList();
        SizeX = HeightmapRows.First().Length;
        SizeY = HeightmapRows.Count;
        Size = SizeY * SizeX;
        Map = map;
    }
}