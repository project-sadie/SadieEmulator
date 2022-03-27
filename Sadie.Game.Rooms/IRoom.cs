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
    public string Description { get; }
    public int Score { get; }
    public List<string> Tags { get; }
    public int MaxUsers { get; }
    IRoomUserRepository UserRepository { get; }
    public IRoomSettings Settings { get; }
}