using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Users;
using Sadie.Db;
using Sadie.Db.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class GateInteractor(
    IDbContextFactory<SadieContext> dbContextFactory,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => [FurnitureItemInteractionType.Gate];
    
    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        if (room.TileMap.UsersAtPoint(new Point(item.PositionX, item.PositionY)) || 
            room
                .UserRepository
                .GetAll()
                .Any(x => x.IsWalking && x.NextPoint == new Point(item.PositionX, item.PositionY)))
        {
            return; 
        }
        
        var newState = item.PlayerFurnitureItem.MetaData == "0" ? 1 : 0;
        
        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(
            room, 
            item, 
            newState.ToString());

        room.TileMap.Map[item.PositionY, item.PositionX] = (short) (newState == 1 ? 1 : 0);
    }

    public override async Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "0");
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(item.PlayerFurnitureItem!).Property(x => x.MetaData).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}