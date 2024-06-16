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
    public int SizeX { get; }
    public int SizeY { get; }
    public int Size { get; }
    public short[,] Map { get; }
    public ConcurrentDictionary<Point, List<IRoomUser>> UserMap { get; } = [];
    public ConcurrentDictionary<Point, List<IRoomBot>> BotMap { get; } = [];
    public short[,] ZMap { get; set; }
    public short[,] TileExistenceMap { get; set; }

    public RoomTileMap(string heightmap, ICollection<PlayerFurnitureItemPlacementData> furnitureItems)
    {
        var heightmapLines = heightmap.Replace("\n", "").Split("\r").ToList();
        
        SizeX = heightmapLines[0].Length;
        SizeY = heightmapLines.Count;
        Size = SizeY * SizeX;
        Map = new short[SizeY, SizeX];
        ZMap = new short[SizeY, SizeX];
        TileExistenceMap = new short[SizeY, SizeX];
        
        for (var y = 0; y < SizeY; y++)
        {
            for (var x = 0; x < SizeX; x++)
            {
                if (heightmapLines[y].Length != SizeX)
                {
                    break;
                }

                var square = heightmapLines[y][x].ToString().ToUpper();
                
                if (square == "X")
                {
                    continue;
                }

                if (!short.TryParse(square, out var height))
                {
                    height = (short) (10 + "ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(square));
                }

                Map[y, x] = RoomTileMapHelpers.GetStateNumberForTile(x, y, furnitureItems);
                ZMap[y, x] = height;
                TileExistenceMap[y, x] = 1;
            }
        }
    }

    public void AddUserToMap(Point point, IRoomUser user) => 
        UserMap.GetOrInsert(point, () => []).Add(user);

    public void AddBotToMap(Point point, IRoomBot bot) => 
        BotMap.GetOrInsert(point, () => []).Add(bot);

    public bool IsTileFree(Point point) => 
        UserMap[point].Count < 1 && BotMap[point].Count < 1;
}