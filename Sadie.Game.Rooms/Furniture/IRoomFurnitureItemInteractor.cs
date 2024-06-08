using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Rooms.Furniture;

namespace Sadie.Game.Rooms.Furniture;

public interface IRoomFurnitureItemInteractor
{
    string InteractionType { get; }
    Task OnClickAsync(RoomLogic room, RoomFurnitureItem item, IRoomUser roomUser);
}