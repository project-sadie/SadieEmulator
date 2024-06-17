using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class OneWayGateInteractor(SadieContext dbContext) : IRoomFurnitureItemInteractor
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

        if (!room.TileMap.TileExists(squareBehind))
        {
            return;
        }

        var itemPoint = new Point(item.PositionX, item.PositionY);
        
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "1");

        roomUnit.DirectionHead = RoomTileMapHelpers.GetOppositeDirection((int) item.Direction);
        roomUnit.Direction = RoomTileMapHelpers.GetOppositeDirection((int) item.Direction);
        roomUnit.OverridePoints.Add(itemPoint);
        roomUnit.CanWalk = false;
        roomUnit.WalkToPoint(squareBehind, OnReachedGoal);
        
        return;

        async void OnReachedGoal()
        {
            await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "0");

            roomUnit.OverridePoints.Remove(itemPoint);
            roomUnit.CanWalk = true;
        }
    }

    public async Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "0");
        dbContext.Entry(item.PlayerFurnitureItem!).Property(x => x.MetaData).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
    
    public Task OnPickUpAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit) => Task.CompletedTask;
    public Task OnMoveAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit) => Task.CompletedTask;
    public Task OnStepOnAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit? roomUnit) => Task.CompletedTask;
    public Task OnStepOffAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit? roomUnit) => Task.CompletedTask;
}