using System.Collections.Concurrent;
using Sadie.Game.Rooms.Packets;

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

    public bool TryRemove(long id)
    {
        var result = _users.TryRemove(id, out _);

        if (result)
        {
            BroadcastDataAsync(new RoomUserLeftWriter(id).GetAllBytes()).Wait(); // TODO: Sort this
        }

        return result;
    }
    public int Count => _users.Count;
    
    public async Task BroadcastDataAsync(byte[] data)
    {
        foreach (var roomUser in _users.Values)
        {
            await roomUser.NetworkObject.WriteToStreamAsync(data);
        }
    }
    
    public async Task UpdateStatusForUsersAsync()
    {
        var writer = new RoomUserStatusWriter(GetAll());
        await BroadcastDataAsync(writer.GetAllBytes());
    }

    public void Dispose()
    {
        foreach (var user in _users.Values)
        {
            user.Dispose();
        }
        
        _users.Clear();
    }
}