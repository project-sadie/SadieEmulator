using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms;

public class RoomLayoutData
{
    public List<string> HeightmapRows { get; }
    public int SizeX { get; }
    public int SizeY { get; }
    public int Size { get; }
    public List<RoomTile> Tiles { get; }
    public short[,] TileMap { get; }

    protected RoomLayoutData(string heightmap, List<RoomTile> tiles)
    {
        HeightmapRows = heightmap.Split("\n").ToList();
        SizeX = HeightmapRows.First().Length;
        SizeY = HeightmapRows.Count;
        Size = SizeY * SizeX;
        Tiles = tiles;
        TileMap = new short[SizeY, SizeX];
    }

    public RoomTile? FindTile(int x, int y) => Tiles.FirstOrDefault(tile => tile.Point.X == x && tile.Point.Y == y);
}