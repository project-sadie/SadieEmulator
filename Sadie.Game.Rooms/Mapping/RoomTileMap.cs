using System.Collections.Concurrent;
using System.Drawing;
using Sadie.API.Game.Rooms.Bots;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Shared.Extensions;

namespace Sadie.Game.Rooms.Mapping;

public class RoomTileMap : IRoomTileMap
{
    private List<string> HeightmapRows { get; }
    public int SizeX { get; }
    public int SizeY { get; }
    public int Size { get; }
    public short[,] Map { get; }
    public ConcurrentDictionary<Point, List<IRoomUser>> UserMap { get; } = [];
    public ConcurrentDictionary<Point, List<IRoomBot>> BotMap { get; } = [];

    public RoomTileMap(string heightmap, ICollection<PlayerFurnitureItemPlacementData> furnitureItems)
    {
        HeightmapRows = heightmap.Split("\r\n").ToList();
        SizeX = HeightmapRows.MaxBy(x => x.Length)!.Length;
        SizeY = HeightmapRows.Count;
        Size = SizeY * SizeX;
        Map = RoomTileMapHelpers.BuildTileMapForRoom(SizeX, SizeY, heightmap, furnitureItems);
    }

    public void AddUserToMap(Point point, IRoomUser user) => 
        UserMap.GetOrInsert(point, () => []).Add(user);

    public void AddBotToMap(Point point, IRoomBot bot) => 
        BotMap.GetOrInsert(point, () => []).Add(bot);
}