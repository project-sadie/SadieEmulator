using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Rooms.Furniture;

namespace Sadie.API.Game.Rooms.Furniture;

public interface IRoomFurnitureItemInteractor
{
    string InteractionType { get; }
    Task OnTriggerAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUser roomUser);
    Task OnPlaceAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUser roomUser);
    Task OnMoveAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUser roomUser);
}