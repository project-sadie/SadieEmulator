using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Rooms.Furniture;

namespace Sadie.API.Game.Rooms;

public interface IRoomLogic
{
    int Id { get; }
    ICollection<RoomFurnitureItem> FurnitureItems { get; }
    IRoomUserRepository UserRepository { get; }
}