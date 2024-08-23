using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Shared;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class DiceInteractor : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => ["dice"];
    
    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, "-1");
        await Task.Delay(1500);
        await RoomFurnitureItemHelpers.UpdateMetaDataForItemAsync(room, item, GlobalState.Random.Next(1, 6).ToString());
    }
}