using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms.Packets.Writers;

namespace Sadie.Game.Rooms.Users;

public class RoomUserRepository(ILogger<RoomUserRepository> logger) : IRoomUserRepository
{
    private readonly ConcurrentDictionary<int, IRoomUser> _users = new();

    public ICollection<IRoomUser> GetAll() => _users.Values;
    public bool TryAdd(IRoomUser user) => _users.TryAdd(user.Id, user);
    public bool TryGet(int id, out IRoomUser? user) => _users.TryGetValue(id, out user);
    public bool TryGetById(long id, out IRoomUser? user)
    {
        user = _users.Values.FirstOrDefault(x => x.Id == id);
        return user != null;
    }

    public bool TryGetByUsername(string username, out IRoomUser? user)
    {
        user = _users.Values.FirstOrDefault(x => x.AvatarData.Username == username);
        return user != null;
    }

    public async Task TryRemoveAsync(int id, bool hotelView = false)
    {
        await BroadcastDataAsync(new RoomUserLeftWriter(id).GetAllBytes());

        var result = _users.TryRemove(id, out var roomUser);

        if (!result || roomUser == null)
        {
            logger.LogError($"Failed to remove a room user");
            return;
        }
        
        if (hotelView)
        {
            await roomUser.NetworkObject.WriteToStreamAsync(new PlayerHotelViewWriter().GetAllBytes());
        }
        
        await roomUser.DisposeAsync();
    }
    
    public int Count => _users.Count;
    
    public async Task BroadcastDataAsync(byte[] data)
    {
        foreach (var roomUser in _users.Values)
        {
            await roomUser.NetworkObject.WriteToStreamAsync(data);
        }
    }

    public ICollection<IRoomUser> GetAllWithRights()
    {
        return _users.Values.Where(x => x.HasRights()).ToList();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var user in _users.Values)
        {
            await TryRemoveAsync(user.Id);
        }
        
        _users.Clear();
    }
}