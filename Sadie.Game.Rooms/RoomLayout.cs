using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms;

public class RoomLayout : RoomLayoutData
{
    public int Id { get; }
    public string Name { get; }
    public string HeightMap { get; }
    public short[,] TileMap { get; }
    
    public RoomLayout(
        int id, 
        string name, 
        string heightMap, 
        HPoint doorPoint, 
        HDirection doorDirection, 
        List<RoomTile> tiles) : base(heightMap, doorPoint, doorDirection, tiles)
    {
        Id = id;
        Name = name;
        HeightMap = heightMap;
        TileMap = new short[SizeY, SizeX];

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
            
            TileMap[point.Y, point.X] = (short)(tile.State == RoomTileState.Open && canWalkOnTile ? 1 : 0);
        }
    }
}