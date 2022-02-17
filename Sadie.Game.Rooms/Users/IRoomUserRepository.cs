namespace Sadie.Game.Rooms.Users;

public interface IRoomUserRepository
{
    ICollection<RoomUser> GetAll();
    bool TryAdd(RoomUser user);
    bool TryGet(long id, out RoomUser? user);
    bool TryRemove(long id);
    int Count { get; }
    Task BroadcastDataToUsersAsync(byte[] data);
}