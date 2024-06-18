using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class GateInteractor(SadieContext dbContext) : AbstractRoomFurnitureItemInteractor
{
    public override string InteractionType => "gate";
    
    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        if (!room.TileMap.IsTileFree(new Point(item.PositionX, item.PositionY)))
        {
            return;
        }

        if (room
            .UserRepository
            .GetAll()
            .Any(x => x.IsWalking && x.NextPoint == new Point(item.PositionX, item.PositionY)))
        {
            return;
        }
        
        var newState = item.PlayerFurnitureItem.MetaData == "0" ? 1 : 0;
        
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(
            room, 
            item, 
            newState.ToString());

        room.TileMap.Map[item.PositionY, item.PositionX] = (short) (newState == 1 ? 1 : 0);
    }

    public override async Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "0");
        dbContext.Entry(item.PlayerFurnitureItem!).Property(x => x.MetaData).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}