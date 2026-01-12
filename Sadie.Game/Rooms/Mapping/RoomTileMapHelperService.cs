using System.Drawing;
using Sadie.API.DTOs.Players.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Mapping;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Enums.Game.Rooms.Mapping;
using Sadie.Core.Enums.Game.Rooms.Users;
using Sadie.Core.Enums.Miscellaneous;

namespace Sadie.Game.Rooms.Mapping;

public class RoomTileMapHelperService : IRoomTileMapHelperService
{
    public HDirection GetOppositeDirection(HDirection direction)
    {
        return direction switch
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
    
    public List<Point> GetPointsForPlacement(
        int x, 
        int y, 
        int width, 
        int length, 
        HDirection direction)
    {
        var points = new List<Point>();
        
        switch (direction)
        {
            case HDirection.North or HDirection.South:
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
            case HDirection.East or HDirection.West:
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

    public RoomTileState GetTileState(
        int x, 
        int y, 
        IEnumerable<PlayerFurnitureItemPlacementDataDto> furnitureItems)
    {
        var item = GetItemsForPosition(x, y, furnitureItems).MaxBy(x => x.PositionZ);

        if (item == null)
        {
            return RoomTileState.Open;
        }
        
        var furnitureItem = item.PlayerFurnitureItem.FurnitureItem;

        if (furnitureItem.CanWalk)
        {
            return RoomTileState.Open;
        }
        
        if (furnitureItem.CanSit)
        {
            return RoomTileState.Sit;
        }

        if (furnitureItem.InteractionType == FurnitureItemInteractionType.Gate && 
            item.PlayerFurnitureItem.MetaData == "1")
        {
            return RoomTileState.Open;
        }

        return furnitureItem.CanLay ? RoomTileState.Lay : RoomTileState.Blocked;
    }

    public List<PlayerFurnitureItemPlacementDataDto> GetItemsForPosition(int x,
        int y,
        IEnumerable<PlayerFurnitureItemPlacementDataDto> items)
    {
        var tileItems = new List<PlayerFurnitureItemPlacementDataDto>();
        
        foreach (var item in items)
        {
            var width = 0;
            var length = 0;

            var furnitureItem = item.PlayerFurnitureItem.FurnitureItem;
            
            if (furnitureItem.Type != FurnitureItemType.Floor)
            {
                continue;
            }

            switch (item.Direction)
            {
                case HDirection.East or HDirection.West:
                    width = furnitureItem.TileSpanY > 0 ? furnitureItem.TileSpanY : 1;
                    length = furnitureItem.TileSpanX > 0 ? furnitureItem.TileSpanX : 1;
                    break;
                case HDirection.North or HDirection.South:
                    width = furnitureItem.TileSpanX > 0 ? furnitureItem.TileSpanX : 1;
                    length = furnitureItem.TileSpanY > 0 ? furnitureItem.TileSpanY : 1;
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
    
    public short[,] GetWorldArrayFromTileMap(IRoomTileMap map,
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

                if (map.UnitMap.TryGetValue(new Point(x, y), out var users) && users.Count > 0)
                {
                    tmp[y, x] = 0;
                    continue;
                }
                
                tmp[y, x] = map.Map[y, x];
            }
        }

        return tmp;
    }

    public void UpdateTileMapsForPoints(
        List<Point> points, 
        IRoomTileMap tileMap, 
        ICollection<PlayerFurnitureItemPlacementDataDto> furnitureItems)
    {
        foreach (var point in points)
        {
            tileMap.Map[point.Y, point.X] = (short) GetTileState(point.X, point.Y, furnitureItems);
            tileMap.UpdateEffectMapForTile(point.X, point.Y, furnitureItems);
        }
    }

    public bool CanPlaceAt(
        IEnumerable<Point> points,  
        IRoomTileMap tileMap,
        ICollection<PlayerFurnitureItemPlacementDataDto> furnitureItems,
        bool checkForUsers = true)
    {
        foreach (var point in points)
        {
            var topItem = GetItemsForPosition(point.X, point.Y, furnitureItems)
                .MaxBy(x => x.PositionZ);
            
            if (tileMap.Map[point.Y, point.X] == 0 && topItem is { PlayerFurnitureItem.FurnitureItem.CanStack: false })
            {
                return false;
            }

            if (checkForUsers && tileMap.UsersAtPoint(point))
            {
                return false;
            }
        }

        return true;
    }
    
    public List<IRoomUser> GetUsersAtPoints(IEnumerable<Point> points, IEnumerable<IRoomUser> users)
    {
        return users
            .Where(user => points.Contains(user.Point))
            .ToList();
    }

    public Point GetPointInFront(int x, int y, HDirection direction, int offset = 0)
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

    public double GetItemPlacementHeight(
        IRoomTileMap roomTileMap,
        IEnumerable<Point> pointsForPlacement, 
        ICollection<PlayerFurnitureItemPlacementDataDto> roomFurnitureItems)
    {
        if (!pointsForPlacement.Any())
        {
            return default;
        }
        
        var i = new List<PlayerFurnitureItemPlacementDataDto>();
        
        foreach (var p in pointsForPlacement)
        {
            i.AddRange(GetItemsForPosition(p.X, p.Y, roomFurnitureItems));
        }
        
        if (i.Count == 0)
        {
            return pointsForPlacement.Select(x => roomTileMap.ZMap[x.Y, x.X]).Max();
        }

        var highestItem = i.MaxBy(x => x.PositionZ)!;
        return highestItem.PositionZ + highestItem.PlayerFurnitureItem.FurnitureItem.StackHeight;
    }

    public int GetSquaresBetweenPoints(Point a, Point b)
    {
        return Math.Abs(a.X + a.Y - (b.X + b.Y));
    }

    public RoomUserEffect GetEffectFromInteractionType(string interactionType)
    {
        return interactionType switch
        {
            FurnitureItemInteractionType.Water => RoomUserEffect.Swimming,
            _ => 0
        };
    }
}