using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Shared;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class DiceInteractor : IRoomFurnitureItemInteractor
{
    public string InteractionType => "dice";
    
    public async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "-1");
        await Task.Delay(1500);
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, GlobalState.Random.Next(1, 6).ToString());
    }
}