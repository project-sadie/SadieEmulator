using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class OneWayGateInteractor(SadieContext dbContext) : AbstractRoomFurnitureItemInteractor
{
    public override string InteractionType => FurnitureItemInteractionType.OneWayGate;
    
    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        var squareInFront = RoomTileMapHelpers.GetPointInFront(item.PositionX, item.PositionY, item.Direction);
        
        if (roomUser.Point != squareInFront)
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

        roomUser.DirectionHead = RoomTileMapHelpers.GetOppositeDirection((int) item.Direction);
        roomUser.Direction = RoomTileMapHelpers.GetOppositeDirection((int) item.Direction);
        roomUser.OverridePoints.Add(itemPoint);
        roomUser.CanWalk = false;
        roomUser.WalkToPoint(squareBehind, OnReachedGoal);
        
        return;

        async void OnReachedGoal()
        {
            await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "0");

            roomUser.OverridePoints.Remove(itemPoint);
            roomUser.CanWalk = true;
        }
    }

    public override async Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "0");
        dbContext.Entry(item.PlayerFurnitureItem!).Property(x => x.MetaData).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}