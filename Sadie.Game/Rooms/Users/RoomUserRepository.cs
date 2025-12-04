using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Networking.Packets.Serialization;
using Sadie.Networking.Packets.Writers.Rooms;
using Sadie.Networking.Packets.Writers.Rooms.Bots;
using Sadie.Networking.Packets.Writers.Rooms.Users;

namespace Sadie.Game.Rooms.Users;

public class RoomUserRepository(ILogger<RoomUserRepository> logger,
    IPlayerRepository playerRepository,
    IPlayerHelperService playerHelperService) : IRoomUserRepository
{
    private readonly ConcurrentDictionary<long, IRoomUser> _users = new();

    public ICollection<IRoomUser> GetAll() => _users.Values;
    
    public bool TryAdd(IRoomUser user) => _users.TryAdd(user.Player.Player.Id, user);
    
    public bool TryGetById(long id, out IRoomUser? user) => _users.TryGetValue(id, out user);

    public bool TryGetByUsername(string username, out IRoomUser? user)
    {
        user = _users.Values.FirstOrDefault(x => x.Player.Player.Username == username);
        return user != null;
    }
    
    private IRoomLogic _room = null!;
    public DateTime? NoUsersSince { get; set; }

    public void SetRoom(IRoomLogic room)
    {
        _room = room;
    }

    public async Task TryRemoveAsync(
        long id, 
        bool notifyLeft, 
        bool hotelView = false)
    {
        var result = _users.TryRemove(id, out var roomUser);

        if (!result || roomUser == null)
        {
            logger.LogError($"Failed to remove a room user");
            return;
        }
        
        if (notifyLeft)
        {
            var writer = new RoomUserLeftWriter
            {
                UserId = id.ToString()
            };

            await _room.BroadcastDataAsync(writer);
        }
        
        var player = roomUser.Player;
        player.State.CurrentRoomId = 0;
        
        await playerHelperService.UpdatePlayerStatusForFriendsAsync(
            player,
            player.GetMergedFriendships(),
            true, 
            false,
            playerRepository);
        
        if (hotelView)
        {
            await roomUser.NetworkObject.WriteToStreamAsync(new RoomUserHotelViewWriter());
        }
        
        await roomUser.DisposeAsync();
    }
    
    public int Count => _users.Count;

    public ICollection<IRoomUser> GetAllWithRights()
    {
        return _users.Values.Where(x => x.HasRights()).ToList();
    }
    
    public async Task RunPeriodicCheckAsync()
    {
        try
        {
            var users = _users.Values;

            if (users.Count == 0)
            {
                NoUsersSince ??= DateTime.Now;
            }
            else
            {
                NoUsersSince = null;
            }
            
            var userCheckTasks = users.Select(user => user.RunPeriodicCheckAsync()).ToList();
            await Task.WhenAll(userCheckTasks);

            var bots = users.First().Room.BotRepository.GetAll();
            
            if (bots.Count > 0)
            {
                await _room.BroadcastDataAsync(new RoomBotStatusWriter { Bots = bots });
                await _room.BroadcastDataAsync(new RoomBotDataWriter { Bots = bots });
            }

            var usersNeedsUpdate = users
                .Where(x => x.NeedsUpdate)
                .ToList();

            if (usersNeedsUpdate.Any())
            {
                var dataWriter = NetworkPacketWriterSerializer.Serialize(
                    new RoomUserDataWriter
                    {
                        Users = usersNeedsUpdate
                    });

                var statusWriter = NetworkPacketWriterSerializer.Serialize(
                    new RoomUserStatusWriter
                    {
                        Users = usersNeedsUpdate
                    });

                foreach (var u in usersNeedsUpdate)
                {
                    await u.NetworkObject.WriteToStreamAsync(dataWriter);
                    await u.NetworkObject.WriteToStreamAsync(statusWriter);
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var user in _users.Values)
        {
            await TryRemoveAsync(user.Player.Player.Id, false);
        }
        
        _users.Clear();
    }
}