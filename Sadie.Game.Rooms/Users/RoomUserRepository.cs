using System.Collections.Concurrent;

namespace Sadie.Game.Rooms.Users;

public class RoomUserRepository : IRoomUserRepository
{
    private readonly ConcurrentDictionary<long, RoomUser> _users;

    public RoomUserRepository()
    {
        _users = new ConcurrentDictionary<long, RoomUser>();
    }

    public ICollection<RoomUser> GetAll() => _users.Values;
    public bool TryAdd(RoomUser user) => _users.TryAdd(user.Id, user);
    public bool TryGet(long id, out RoomUser? user) => _users.TryGetValue(id, out user);
    public bool TryRemove(long id) => _users.TryRemove(id, out var _);
    public int Count => _users.Count;
    
    public async Task BroadcastDataToUsersAsync(byte[] data)
    {
        foreach (var roomUser in _users.Values)
        {
            await roomUser.NetworkObject.WriteToStreamAsync(data);
        }
    }
}