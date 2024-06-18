using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class OneWayGateInteractor(SadieContext dbContext) : AbstractRoomFurnitureItemInteractor
{
    public override string InteractionType => "onewaygate";
    
    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
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

    public override async Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "0");
        dbContext.Entry(item.PlayerFurnitureItem!).Property(x => x.MetaData).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}