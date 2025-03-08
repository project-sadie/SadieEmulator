using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class DefaultInteractor(IRoomWiredService roomWiredService) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => ["*"];

    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        
        var matchingWiredTriggers = roomWiredService.GetTriggers(
            FurnitureItemInteractionType.WiredTriggerFurnitureStateChanged,
            room.FurnitureItems,"", [item.Id]);

        foreach (var trigger in matchingWiredTriggers)
        {
            await roomWiredService.RunTriggerForRoomAsync(room, trigger, roomUser!);
        }
    }
}