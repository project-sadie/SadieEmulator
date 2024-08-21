using System.Drawing;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Game.Rooms.Users;
using Sadie.Enums.Unsorted;

namespace Sadie.Game.Rooms.Mapping;

public class RoomTileMapHelpers
{
    public static HDirection GetOppositeDirection(int direction)
    {
        return (HDirection) direction switch
        {
            HDirection.North => HDirection.South,
            HDirection.NorthEast => HDirection.SouthWest,
            HDirection.East => HDirection.West,
            HDirection.SouthEast => HDirection.NorthWest,
            HDirection.South => HDirection.North,
            HDirection.SouthWest => HDirection.NorthEast,
            HDirection.West => HDirection.East,
            HDirection.NorthWest => HDirection.SouthEast,
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
    }
    
    public static List<Point> GetPointsForPlacement(
        int x, 
        int y, 
        int width, 
        int length, 
        int direction)
    {
        var points = new List<Point>();
        
        switch (direction)
        {
            case 0 or 4:
            {
                for (var i = x; i <= x + (width - 1); i++)
                {
                    for (var j = y; j <= y + (length - 1); j++)
                    {
                        points.Add(new Point(i, j));
                    }
                }

                break;
            }
            case 2 or 6:
            {
                for (var i = x; i <= x + (length - 1); i++)
                {
                    for (var j = y; j <= y + (width - 1); j++)
                    {
                        points.Add(new Point(i, j));
                    }
                }

                break;
            }
        }

        return points;
    }

    public static RoomTileState GetTileState(
        int x, 
        int y, 
        IEnumerable<PlayerFurnitureItemPlacementData> furnitureItems)
    {
        var item = GetItemsForPosition(x, y, furnitureItems).MaxBy(x => x.PositionZ);

        if (item == null ||
            item.FurnitureItem.CanWalk)
        {
            return RoomTileState.Open;
        }
        
        if (item.FurnitureItem.CanSit)
        {
            return RoomTileState.Sit;
        }

        if (item.FurnitureItem.InteractionType == FurnitureItemInteractionType.Gate && item.PlayerFurnitureItem.MetaData == "1")
        {
            return RoomTileState.Open;
        }

        return item.FurnitureItem.CanLay ? RoomTileState.Lay : RoomTileState.Blocked;
    }

    public static List<PlayerFurnitureItemPlacementData> GetItemsForPosition(int x,
        int y,
        IEnumerable<PlayerFurnitureItemPlacementData> items)
    {
        var tileItems = new List<PlayerFurnitureItemPlacementData>();
        
        foreach (var item in items)
        {
            var width = 0;
            var length = 0;
            
            if (item.FurnitureItem.Type != FurnitureItemType.Floor)
            {
                continue;
            }

            switch ((int)item.Direction)
            {
                case 2 or 6:
                    width = item.FurnitureItem.TileSpanY > 0 ? item.FurnitureItem.TileSpanY : 1;
                    length = item.FurnitureItem.TileSpanX > 0 ? item.FurnitureItem.TileSpanX : 1;
                    break;
                case 0 or 4:
                    width = item.FurnitureItem.TileSpanX > 0 ? item.FurnitureItem.TileSpanX : 1;
                    length = item.FurnitureItem.TileSpanY > 0 ? item.FurnitureItem.TileSpanY : 1;
                    break;
            }
            
            if (!(x >= item.PositionX && x <= item.PositionX + width - 1 &&
                  y >= item.PositionY && y <= item.PositionY + length - 1))
            {
                continue;
            }

            tileItems.Add(item);
        }

        return tileItems;
    }
    
    public static short[,] GetWorldArrayFromTileMap(IRoomTileMap map,
        Point goalPoint,
        List<Point> overridePoints)
    {
        var tmp = new short[map.SizeY, map.SizeX];
        
        for (var y = 0; y < map.SizeY; y++)
        {
            for (var x = 0; x < map.SizeX; x++)
            {
                if (overridePoints.Count > 0 && overridePoints.Contains(new Point(x, y)))
                {
                    tmp[y, x] = 1;
                    continue;
                }
                
                // If it's a sit or lay tile, don't include it unless it's our goal
                
                if ((map.Map[y, x] == 2 || map.Map[y, x] == 3) && (goalPoint.X != x || goalPoint.Y != y))
                {
                    tmp[y, x] = 0;
                    continue;
                }
                
                // If the tile has other users on it skip it

                if (map.UserMap.TryGetValue(new Point(x, y), out var users) && users.Count > 0)
                {
                    tmp[y, x] = 0;
                    continue;
                }
                
                tmp[y, x] = map.Map[y, x];
            }
        }

        return tmp;
    }

    public static void UpdateTileMapsForPoints(
        List<Point> points, 
        IRoomTileMap tileMap, 
        ICollection<PlayerFurnitureItemPlacementData> furnitureItems)
    {
        foreach (var point in points)
        {
            tileMap.Map[point.Y, point.X] = (short) GetTileState(point.X, point.Y, furnitureItems);
            tileMap.UpdateEffectMapForTile(point.X, point.Y, furnitureItems);
        }
    }

    public static bool CanPlaceAt(
        IEnumerable<Point> points,  
        IRoomTileMap tileMap)
    {
        return points.All(point => tileMap.Map[point.Y, point.X] != 0);
    }
    
    public static List<IRoomUser> GetUsersAtPoints(IEnumerable<Point> points, IEnumerable<IRoomUser> users)
    {
        return users
            .Where(user => points.Contains(user.Point))
            .ToList();
    }

    public static Point GetPointInFront(int x, int y, HDirection direction, int offset = 0)
    {
        var offsetX = 0;
        var offsetY = 0;
        
        switch ((int) direction % 8) {
            case 0:
                offsetY--;
                break;
            case 1:
                offsetX++;
                offsetY--;
                break;
            case 2:
                offsetX++;
                break;
            case 3:
                offsetX++;
                offsetY++;
                break;
            case 4:
                offsetY++;
                break;
            case 5:
                offsetX--;
                offsetY++;
                break;
            case 6:
                offsetX--;
                break;
            case 7:
                offsetX--;
                offsetY--;
                break;
        }

        for (var i = 0; i <= offset; i++) 
        {
            x += offsetX;
            y += offsetY;
        }

        return new Point(x, y);
    }

    public static double GetItemPlacementHeight(
        IRoomTileMap roomTileMap,
        IEnumerable<Point> pointsForPlacement, 
        ICollection<PlayerFurnitureItemPlacementData> roomFurnitureItems)
    {
        if (!pointsForPlacement.Any())
        {
            return default;
        }
        
        var i = new List<PlayerFurnitureItemPlacementData>();
        
        foreach (var p in pointsForPlacement)
        {
            i.AddRange(GetItemsForPosition(p.X, p.Y, roomFurnitureItems));
        }
        
        if (i.Count == 0)
        {
            return pointsForPlacement.Select(x => roomTileMap.ZMap[x.Y, x.X]).Max();
        }

        var highestItem = i.MaxBy(x => x.PositionZ)!;
        return highestItem.PositionZ + highestItem.FurnitureItem.StackHeight;
    }

    public static int GetSquaresBetweenPoints(Point a, Point b)
    {
        return Math.Abs(a.X + a.Y - (b.X + b.Y));
    }

    public static RoomUserEffect GetEffectFromInteractionType(string interactionType)
    {
        return interactionType switch
        {
            FurnitureItemInteractionType.Water => RoomUserEffect.Swimming,
            _ => 0
        };
    }
}