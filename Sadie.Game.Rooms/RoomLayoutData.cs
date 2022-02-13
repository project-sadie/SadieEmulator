using System.Text;
using Sadie.Shared;

namespace Sadie.Game.Rooms;

public class RoomLayoutData
{
    public List<string> HeightmapRows { get; }
    public int SizeX { get; }
    public int SizeY { get; }
    public int Size { get; }
    public List<RoomTile> Tiles { get; }

    protected RoomLayoutData(string heightmap)
    {
        HeightmapRows = heightmap.Split("\n").ToList();
        SizeX = HeightmapRows.First().Length;
        SizeY = HeightmapRows.Count;
        Size = SizeY * SizeX;
        Tiles = RoomHelpers.BuildTileListFromHeightMap(HeightmapRows);
    }

    public RoomTile? GetTile(int x, int y) => Tiles.FirstOrDefault(tile => tile.Point.X == x && tile.Point.Y == y);
}