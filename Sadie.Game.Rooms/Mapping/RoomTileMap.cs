using System.Collections.Concurrent;
using System.Drawing;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Mapping;

public class RoomTileMap
{
    private List<string> HeightmapRows { get; }
    public int SizeX { get; }
    public int SizeY { get; }
    public int Size { get; }
    public short[,] SquareStateMap { get; }
    public short[,] Map { get; }
    public ConcurrentDictionary<Point, List<IRoomUser>?> UserMap { get; } = [];

    public RoomTileMap(string heightmap, ICollection<RoomFurnitureItem> furnitureItems)
    {
        HeightmapRows = heightmap.Split("\n").ToList();
        SizeX = HeightmapRows.First().Length;
        SizeY = HeightmapRows.Count;
        Size = SizeY * SizeX;
        SquareStateMap = RoomTileMapHelpers.BuildSquareStateMapForRoom(SizeX, SizeY, heightmap);
        Map = RoomTileMapHelpers.BuildTileMapForRoom(SizeX, SizeY, heightmap, furnitureItems);
    }

    public void AddUserToMap(Point point, IRoomUser user)
    {
        if (UserMap.TryGetValue(point, out var value))
        {
            value.Add(user);
        }
        else
        {
            UserMap[point] = [user];
        }
    }
}