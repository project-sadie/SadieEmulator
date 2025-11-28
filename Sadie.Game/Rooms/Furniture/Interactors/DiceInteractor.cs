using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Core.Shared;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class DiceInteractor(
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService)
    : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => ["dice"];
    
    public override async Task OnTriggerAsync(IRoomLogic room, 
        PlayerFurnitureItemPlacementDataDto item, 
        IRoomUser roomUser)
    {
        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, 
            item, "-1");
        
        await Task.Delay(1500);
        
        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room,
            item,
            GlobalState.Random.Next(1,
                    6)
                .ToString());
    }
}