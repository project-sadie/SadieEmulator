using Sadie.Game.Rooms.Chat;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public interface IRoomData
{
    int Id { get; }
    string Name { get; }
    RoomLayout Layout { get; }
    int OwnerId { get; }
    string OwnerName { get; }
    string Description { get; }
    int Score { get; }
    List<string> Tags { get; }
    int MaxUsers { get; }
    bool Muted { get; }
    IRoomUserRepository UserRepository { get; }
    IRoomSettings Settings { get; }
    List<RoomChatMessage> ChatMessages { get; }
    public List<int> PlayersWithRights { get; }
}