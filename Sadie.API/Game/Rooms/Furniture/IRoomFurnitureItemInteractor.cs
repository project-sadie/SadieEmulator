using Sadie.API.Game.Rooms.Unit;
using Sadie.Database.Models.Players.Furniture;

namespace Sadie.API.Game.Rooms.Furniture;

public interface IRoomFurnitureItemInteractor
{
    string InteractionType { get; }
    Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit);
    Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit);
    Task OnPickUpAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit);
    Task OnMoveAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit);
}