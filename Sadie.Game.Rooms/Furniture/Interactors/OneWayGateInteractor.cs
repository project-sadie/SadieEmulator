using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class OneWayGateInteractor : IRoomFurnitureItemInteractor
{
    public string InteractionType => "onewaygate";
    
    public async Task OnTriggerAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUser roomUser)
    {
        var squareInFront = RoomTileMapHelpers.GetPointInFront(item.PositionX, item.PositionY, item.Direction);
        
        if (roomUser.Point != squareInFront)
        {
            return;
        }

        var itemPoint = new Point(item.PositionX, item.PositionY);
        
        item.MetaData = "1";
        await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, item);

        roomUser.DirectionHead = RoomTileMapHelpers.GetOppositeDirection((int) item.Direction);
        roomUser.Direction = RoomTileMapHelpers.GetOppositeDirection((int) item.Direction);

        var squareBehind = RoomTileMapHelpers.GetPointInFront(item.PositionX, item.PositionY,
            RoomTileMapHelpers.GetOppositeDirection((int) item.Direction));

        roomUser.OverridePoints.Add(itemPoint);
        
        roomUser.WalkToPoint(squareBehind, async () =>
        {
            item.MetaData = "0";
            await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, item);
            
            roomUser.OverridePoints.Remove(itemPoint);
        });
    }

    public Task OnPlaceAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUser roomUser) => Task.CompletedTask;
    public Task OnMoveAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUser roomUser) => Task.CompletedTask;
}