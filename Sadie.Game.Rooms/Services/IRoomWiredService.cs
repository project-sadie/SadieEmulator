using Sadie.Database.Models.Players.Furniture;

namespace Sadie.Game.Rooms.Services;

public interface IRoomWiredService
{
    Task RunTriggerAsync(PlayerFurnitureItemPlacementData trigger,
        IEnumerable<PlayerFurnitureItemPlacementData> roomItems);
}