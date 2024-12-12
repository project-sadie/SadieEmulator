using System.Drawing;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Rooms.Mapping;
using Sadie.Enums.Game.Rooms.Users;
using Sadie.Enums.Unsorted;

namespace Sadie.API.Game.Rooms.Mapping;

public interface IRoomTileMapHelperService
{
    HDirection GetOppositeDirection(int direction);

    List<Point> GetPointsForPlacement(
        int x, 
        int y, 
        int width, 
        int length, 
        int direction);

    RoomTileState GetTileState(
        int x, 
        int y, 
        IEnumerable<PlayerFurnitureItemPlacementData> furnitureItems);

    List<PlayerFurnitureItemPlacementData> GetItemsForPosition(int x,
        int y,
        IEnumerable<PlayerFurnitureItemPlacementData> items);

    short[,] GetWorldArrayFromTileMap(IRoomTileMap map,
        Point goalPoint,
        List<Point> overridePoints);

    void UpdateTileMapsForPoints(
        List<Point> points, 
        IRoomTileMap tileMap, 
        ICollection<PlayerFurnitureItemPlacementData> furnitureItems);

    bool CanPlaceAt(
        IEnumerable<Point> points,  
        IRoomTileMap tileMap,
        bool checkForUsers = true);

    List<IRoomUser> GetUsersAtPoints(IEnumerable<Point> points, IEnumerable<IRoomUser> users);
    Point GetPointInFront(int x, int y, HDirection direction, int offset = 0);

    double GetItemPlacementHeight(
        IRoomTileMap roomTileMap,
        IEnumerable<Point> pointsForPlacement, 
        ICollection<PlayerFurnitureItemPlacementData> roomFurnitureItems);

    int GetSquaresBetweenPoints(Point a, Point b);
    RoomUserEffect GetEffectFromInteractionType(string interactionType);
}