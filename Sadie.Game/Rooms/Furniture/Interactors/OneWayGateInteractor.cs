using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Users;
using Sadie.Db;
using Sadie.Db.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class OneWayGateInteractor(
    IDbContextFactory<SadieContext> dbContextFactory,
    IRoomTileMapHelperService tileMapHelperService,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => [FurnitureItemInteractionType.OneWayGate];
    
    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        var squareInFront = tileMapHelperService.GetPointInFront(item.PositionX, item.PositionY, item.Direction);
        
        if (roomUser.Point != squareInFront)
        {
            return;
        }

        var squareBehind = tileMapHelperService.GetPointInFront(item.PositionX, item.PositionY,
            tileMapHelperService.GetOppositeDirection((int) item.Direction));

        if (!room.TileMap.TileExists(squareBehind))
        {
            return;
        }

        var itemPoint = new Point(item.PositionX, item.PositionY);
        
        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "1");

        roomUser.DirectionHead = tileMapHelperService.GetOppositeDirection((int) item.Direction);
        roomUser.Direction = tileMapHelperService.GetOppositeDirection((int) item.Direction);
        roomUser.OverridePoints.Add(itemPoint);
        roomUser.CanWalk = false;
        roomUser.WalkToPoint(squareBehind, OnReachedGoal);
        
        return;

        async void OnReachedGoal()
        {
            await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "0");

            roomUser.OverridePoints.Remove(itemPoint);
            roomUser.CanWalk = true;
        }
    }

    public override async Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "0");
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(item.PlayerFurnitureItem!).Property(x => x.MetaData).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}