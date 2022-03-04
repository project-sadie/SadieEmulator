using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public interface IRoom : IDisposable
{
    Task RunPeriodicCheckAsync();
    long Id { get; }
    string Name { get; }
    RoomLayout Layout { get; }
    IRoomUserRepository UserRepository { get; }
}