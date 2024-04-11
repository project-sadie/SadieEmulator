using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms.Tiles;

public class RoomTileMap
{
    private List<string> HeightmapRows { get; }
    
    public int SizeX { get; }
    public int SizeY { get; }
    public int Size { get; }
    public List<RoomTile> Tiles { get; }
    public short[,] Map { get; }

    public RoomTileMap(string heightmap, List<RoomTile> tiles)
    {
        HeightmapRows = heightmap.Split("\n").ToList();
        SizeX = HeightmapRows.First().Length;
        SizeY = HeightmapRows.Count;
        Size = SizeY * SizeX;
        Tiles = tiles;
        Map = new short[SizeY, SizeX];

        GenerateTileMap();
    }

    private void GenerateTileMap()
    {
        foreach (var tile in Tiles)
        {
            var point = tile.Point;
            var topLevelItem = tile.Items.MaxBy(x => x.PositionZ);
            
            var canWalkOnTile = topLevelItem == null ||
                topLevelItem.FurnitureItem.CanSit ||
                topLevelItem.FurnitureItem.CanWalk ||
                topLevelItem.FurnitureItem.CanLay;

            if (tile.Users.Count > 0)
            {
                canWalkOnTile = false;
            }
            
            Map[point.Y, point.X] = (short)(tile.State == RoomTileState.Open && canWalkOnTile ? 1 : 0);
        }
    }

    public RoomTile? GetTile(int x, int y) => Tiles.FirstOrDefault(tile => tile.Point.X == x && tile.Point.Y == y);

    public List<RoomTile> GetTilesForSpan(
        int x, 
        int y, 
        int width, 
        int length, 
        int direction)
    {
        var tiles = new List<RoomTile>();
        
        if (direction is 0 or 4)
        {
            for (var i = x; i <= x + (width - 1); i++)
            {
                for (var j = y; j <= y + (length - 1); j++)
                {
                    var t = GetTile(i, j);

                    if (t == null)
                    {
                        continue;
                    }
                    
                    tiles.Add(t);
                }
            }
        }
        else if (direction is 2 or 6)
        {
            for (var i = x; i <= x + (length - 1); i++)
            {
                for (var j = y; j <= y + (width - 1); j++)
                {
                    var t = GetTile(i, j);

                    if (t == null)
                    {
                        continue;
                    }
                    
                    tiles.Add(t);
                }
            }
        }

        return tiles;
    }
}