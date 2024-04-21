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
    public short[,] Map { get; private set; }
    private ConcurrentDictionary<Point, List<IRoomUser>> _userMap { get; set; } = [];

    public RoomTileMap(string heightmap, short[,] map)
    {
        HeightmapRows = heightmap.Split("\n").ToList();
        SizeX = HeightmapRows.First().Length;
        SizeY = HeightmapRows.Count;
        Size = SizeY * SizeX;
        Map = map;
    }

    public void AddUserToMap(Point point, IRoomUser user)
    {
        if (_userMap.TryGetValue(point, out var value))
        {
            value.Add(user);
        }
        else
        {
            _userMap[point] = [user];
        }
    }

    public void RemoveUserFromMap(Point point, IRoomUser user) => _userMap[point].Remove(user);
    public List<IRoomUser> GetMappedUsers(Point point) => _userMap.TryGetValue(point, out var value) ? value : [];
}