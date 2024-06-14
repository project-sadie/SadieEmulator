using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class OneWayGateInteractor : IRoomFurnitureItemInteractor
{
    public string InteractionType => "onewaygate";
    
    public async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        var squareInFront = RoomTileMapHelpers.GetPointInFront(item.PositionX, item.PositionY, item.Direction);
        
        if (roomUnit.Point != squareInFront)
        {
            return;
        }

        var squareBehind = RoomTileMapHelpers.GetPointInFront(item.PositionX, item.PositionY,
            RoomTileMapHelpers.GetOppositeDirection((int) item.Direction));

        if (room.TileMap.Map[squareBehind.Y, squareBehind.X] == 0)
        {
            return;
        }

        var itemPoint = new Point(item.PositionX, item.PositionY);
        
        item.PlayerFurnitureItem.MetaData = "1";
        await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, item);

        roomUnit.DirectionHead = RoomTileMapHelpers.GetOppositeDirection((int) item.Direction);
        roomUnit.Direction = RoomTileMapHelpers.GetOppositeDirection((int) item.Direction);
        roomUnit.OverridePoints.Add(itemPoint);
        roomUnit.CanWalk = false;
        roomUnit.WalkToPoint(squareBehind, OnReachedGoal);
        
        return;

        async void OnReachedGoal()
        {
            item.PlayerFurnitureItem.MetaData = "0";
            await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, item);

            roomUnit.OverridePoints.Remove(itemPoint);
            roomUnit.CanWalk = true;
        }
    }

    public Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit) => Task.CompletedTask;
    public Task OnPickUpAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit) => Task.CompletedTask;
    public Task OnMoveAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit) => Task.CompletedTask;
    public Task OnStepOnAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit? roomUnit) => Task.CompletedTask;
    public Task OnStepOffAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit? roomUnit) => Task.CompletedTask;
}