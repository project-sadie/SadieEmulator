using System.Collections.Concurrent;
using System.Drawing;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Mapping;

public class RoomTileMap
{
    private List<string> HeightmapRows { get; }
    public int SizeX { get; }
    public int SizeY { get; }
    public int Size { get; }
    public short[,] SquareStateMap { get; private set; }
    public short[,] Map { get; private set; }
    public ConcurrentDictionary<Point, List<IRoomUser>> UserMap { get; } = [];

    public RoomTileMap(string heightmap, short[,] tileMap, short[,] map)
    {
        HeightmapRows = heightmap.Split("\n").ToList();
        SizeX = HeightmapRows.First().Length;
        SizeY = HeightmapRows.Count;
        Size = SizeY * SizeX;
        SquareStateMap = tileMap;
        Map = map;
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

    public void RemoveUserFromMap(Point point, IRoomUser user) =>  UserMap[point].Remove(user);
    public List<IRoomUser> GetMappedUsers(Point point) => UserMap.TryGetValue(point, out var value) ? value : [];
}