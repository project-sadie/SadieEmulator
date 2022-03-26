using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public interface IRoom : IAsyncDisposable
{
    Task RunPeriodicCheckAsync();
    long Id { get; }
    string Name { get; }
    RoomLayout Layout { get; }
    long OwnerId { get; }
    string OwnerName { get; }
    IRoomUserRepository UserRepository { get; }
}