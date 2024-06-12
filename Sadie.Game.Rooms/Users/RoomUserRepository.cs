using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Sadie.API.Game.Rooms.Users;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Serialization;

namespace Sadie.Game.Rooms.Users;

public class RoomUserRepository(ILogger<RoomUserRepository> logger) : IRoomUserRepository
{
    private readonly ConcurrentDictionary<long, IRoomUser> _users = new();

    public ICollection<IRoomUser> GetAll() => _users.Values;
    public bool TryAdd(IRoomUser user) => _users.TryAdd(user.Id, user);
    public bool TryGetById(long id, out IRoomUser? user) => _users.TryGetValue(id, out user);

    public bool TryGetByUsername(string username, out IRoomUser? user)
    {
        user = _users.Values.FirstOrDefault(x => x.Player.Username == username);
        return user != null;
    }

    public async Task TryRemoveAsync(long id, bool hotelView = false)
    {
        var writer = new RoomUserLeftWriter
        {
            UserId = id.ToString()
        };
        
        await BroadcastDataAsync(writer);

        var result = _users.TryRemove(id, out var roomUser);

        if (!result || roomUser == null)
        {
            logger.LogError($"Failed to remove a room user");
            return;
        }
        
        if (hotelView)
        {
            await roomUser.NetworkObject.WriteToStreamAsync(new RoomUserHotelView());
        }
        
        await roomUser.DisposeAsync();
    }
    
    public int Count => _users.Count;
    
    public async Task BroadcastDataAsync(AbstractPacketWriter writer)
    {
        var serializedObject = NetworkPacketWriterSerializer.Serialize(writer);
        
        foreach (var roomUser in _users.Values)
        {
            await roomUser.NetworkObject.WriteToStreamAsync(serializedObject);
        }
    }

    public ICollection<IRoomUser> GetAllWithRights()
    {
        return _users.Values.Where(x => x.HasRights()).ToList();
    }

    public async Task RunPeriodicCheckAsync()
    {
        var users = _users.Values;
        
        foreach (var roomUser in users)
        {
            await roomUser.RunPeriodicCheckAsync();
        }

        var statusWriter = new RoomUserStatusWriter
        {
            Users = users
        };
        
        var dataWriter = new RoomUserDataWriter
        {
            Users = users
        };

        await BroadcastDataAsync(statusWriter);
        await BroadcastDataAsync(dataWriter);
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