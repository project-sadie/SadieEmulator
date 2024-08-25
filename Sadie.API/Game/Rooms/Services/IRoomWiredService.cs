using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Rooms.Furniture;

namespace Sadie.API.Game.Rooms.Services;

public interface IRoomWiredService
{
    IEnumerable<PlayerFurnitureItemPlacementData> GetEffectsForTrigger(
        PlayerFurnitureItemPlacementData trigger,
        IEnumerable<PlayerFurnitureItemPlacementData> roomItems);

    Task RunTriggerForRoomAsync(IRoomLogic room,
        PlayerFurnitureItemPlacementData trigger);

    int GetWiredCode(string interactionType);
}