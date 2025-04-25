using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.API.Game.Rooms.Wired.Effects.Actions;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;

namespace Sadie.Game.Rooms.Wired.Effects.Actions;

public class ToggleRandomFurnitureStateEffectAction(
    SadieContext dbContext,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : IWiredEffectAction
{
    public string InteractionType => FurnitureItemInteractionType.WiredEffectToggleRandomFurnitureState;

    public async Task ExecuteAsync(
        IRoomLogic room, 
        IRoomUser userWhoTriggered, 
        PlayerFurnitureItemPlacementData effect,
        IRoomWiredService wiredService)
    {
        var placementDataIds = effect
                .WiredData!
                .PlayerFurnitureItemWiredItems
                .Select(x => x.PlayerFurnitureItemPlacementDataId);

        foreach (var item in room
                     .FurnitureItems
                     .Where(x => placementDataIds.Contains(x.Id))
                     .Select(x => x)
                     .ToList())
        {
            await roomFurnitureItemHelperService.CycleInteractionStateForItemAsync(
                room, 
                item, 
                dbContext,
                true);
        }
    }
}