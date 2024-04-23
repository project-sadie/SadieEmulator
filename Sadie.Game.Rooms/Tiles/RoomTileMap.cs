using System.Collections.Concurrent;
using System.Drawing;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Tiles;

public class RoomTileMap
{
    private List<string> HeightmapRows { get; }
    
    public int SizeX { get; }
    public int SizeY { get; }
    public int Size { get; }
    public short[,] SquareStateMap { get; private set; }
    public short[,] Map { get; private set; }

    public RoomTileMap(string heightmap, short[,] tileMap, short[,] map)
    {
        HeightmapRows = heightmap.Split("\n").ToList();
        SizeX = HeightmapRows.First().Length;
        SizeY = HeightmapRows.Count;
        Size = SizeY * SizeX;
        SquareStateMap = tileMap;
        Map = map;
    }
}