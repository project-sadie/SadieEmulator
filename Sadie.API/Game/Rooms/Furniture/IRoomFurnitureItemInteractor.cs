using Sadie.API.Game.Rooms.Unit;
using Sadie.Database.Models.Rooms.Furniture;

namespace Sadie.API.Game.Rooms.Furniture;

public interface IRoomFurnitureItemInteractor
{
    string InteractionType { get; }
    Task OnTriggerAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit);
    Task OnPlaceAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit);
    Task OnMoveAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit);
    Task OnStepOnAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit);
    Task OnStepOffAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit);
}