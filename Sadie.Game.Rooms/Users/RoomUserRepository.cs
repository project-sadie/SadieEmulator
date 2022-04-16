using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms.Packets;

namespace Sadie.Game.Rooms.Users;

public class RoomUserRepository : IRoomUserRepository
{
    private readonly ILogger<RoomUserRepository> _logger;
    private readonly ConcurrentDictionary<int, IRoomUser> _users;

    public RoomUserRepository(ILogger<RoomUserRepository> logger)
    {
        _logger = logger;
        _users = new ConcurrentDictionary<int, IRoomUser>();
    }

    public ICollection<IRoomUser> GetAll() => _users.Values;
    public bool TryAdd(IRoomUser user) => _users.TryAdd(user.Id, user);
    public bool TryGet(int id, out IRoomUser? user) => _users.TryGetValue(id, out user);

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
            _logger.LogError($"Failed to remove a room user");
            return;
        }
        
        if (hotelView)
        {
            await roomUser.NetworkObject.WriteToStreamAsync(new PlayerHotelViewWriter().GetAllBytes());
        }
        
        await roomUser.DisposeAsync();
    }
    
    public int Count => _users.Count;
    
    public Task BroadcastDataAsync(byte[] data)
    {
        foreach (var roomUser in _users.Values)
        {
            roomUser.NetworkObject.WriteToStreamAsync(data);
        }

        return Task.CompletedTask;
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