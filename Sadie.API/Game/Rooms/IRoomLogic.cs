using Sadie.API.Game.Rooms.Users;

namespace Sadie.API.Game.Rooms;

public interface IRoomLogic
{
    int Id { get; }
    IRoomUserRepository UserRepository { get; }
}