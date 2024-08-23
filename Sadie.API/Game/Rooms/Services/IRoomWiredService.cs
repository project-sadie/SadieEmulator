using Sadie.Database.Models.Players.Furniture;

namespace Sadie.API.Game.Rooms.Services;

public interface IRoomWiredService
{
    IEnumerable<PlayerFurnitureItemPlacementData> GetEffectsForTrigger(
        PlayerFurnitureItemPlacementData trigger,
        IEnumerable<PlayerFurnitureItemPlacementData> roomItems);

    Task RunTriggerForRoomAsync(IRoomLogic room,
        PlayerFurnitureItemPlacementData trigger);
}