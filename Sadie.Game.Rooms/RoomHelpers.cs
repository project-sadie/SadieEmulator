﻿using System.Drawing;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms.PathFinding;
using Sadie.Game.Rooms.PathFinding.Options;
using Sadie.Game.Rooms.Tiles;
using Sadie.Shared.Extensions;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms;

public static class RoomHelpers
{
    public static List<RoomTile> BuildTileListFromHeightMap(string heightMap, 
        ICollection<RoomFurnitureItem> furnitureItems)
    {
        var heightmapLines = heightMap.Split("\n").ToList();
        var tiles = new List<RoomTile>();
        
        for (var y = 0; y < heightmapLines.Count; y++)
        {
            var currentLine = heightmapLines[y];

            for (var x = 0; x < currentLine.Length; x++)
            {
                var zResult = int.TryParse(currentLine[x].ToString(), out var z);
                
                var state = zResult ? 
                    RoomTileState.Open : 
                    RoomTileState.Closed;

                var items = furnitureItems
                    .Where(item => item.PositionX == x && item.PositionY == y)
                    .ToList();
                
                var tile = new RoomTile(x, y, zResult ? z : 33, state, items);
                
                tiles.Add(tile);
            }
        }
        
        return tiles;
    }

    public static Queue<HPoint> BuildPathForWalk(RoomTileMap tileMap, HPoint start, HPoint end, bool useDiagonal)
    {
        var pathfinderOptions = new PathFinderOptions
        {
            UseDiagonals = useDiagonal
        };
        
        var worldGrid = new WorldGrid(tileMap.Map);
        var pathfinder = new PathFinder(worldGrid, pathfinderOptions);
        var route = pathfinder.FindPath(start.ToPoint(), end.ToPoint()).ToList();
        var points = route.Select(x => tileMap.Tiles.First(y => y.Point.X == x.X && y.Point.Y == x.Y).Point);
        
        return new Queue<HPoint>(points);
    }

    public static HDirection GetDirectionForNextStep(HPoint current, HPoint next)
    {
        var rotation = HDirection.North;

        if (current.X > next.X && current.Y > next.Y)
        {
            rotation = HDirection.NorthWest;
        }
        else if (current.X < next.X && current.Y < next.Y)
        {
            rotation = HDirection.SouthEast;
        }
        else if (current.X > next.X && current.Y < next.Y)
        {
            rotation = HDirection.SouthWest;
        }
        else if (current.X < next.X && current.Y > next.Y)
        {
            rotation = HDirection.NorthEast;
        }
        else if (current.X > next.X)
        {
            rotation = HDirection.West;
        }
        else if (current.X < next.X)
        {
            rotation = HDirection.East;
        }
        else if (current.Y < next.Y)
        {
            rotation = HDirection.South;
        }
        else if (current.Y > next.Y)
        {
            rotation = HDirection.North;
        }

        return rotation;
    }

    public static void UpdateTileMapForTile(RoomTile tile, RoomTileMap tileMap)
    {
        var topLevelItem = tile.Items.MaxBy(x => x.PositionZ);

        if (topLevelItem == null)
        {
            tileMap.Map[tile.Point.Y, tile.Point.X] = 1;
        }
        else
        {
            var furnitureItem = topLevelItem.FurnitureItem;
            var canWalkOnItem = furnitureItem.CanWalk || furnitureItem.CanSit;

            if (tile.Users.Count > 0)
            {
                canWalkOnItem = false;
            }
            
            tileMap.Map[tile.Point.Y, tile.Point.X] = (short)(canWalkOnItem ? 1 : 0);
        }
    }
}