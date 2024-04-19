using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Furniture;

public interface IRoomFurnitureItemInteractor
{
    string InteractionType { get; }
    Task OnClickAsync(RoomLogic room, RoomFurnitureItem item, IRoomUser roomUser);
}