using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.API.Interfaces.Networking;
using Sadie.Networking.Serialization;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Bots;
using Sadie.Networking.Writers.Rooms.Users;

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

            await BroadcastDataAsync(writer, [id]);
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
    
    public async Task BroadcastDataAsync(AbstractPacketWriter writer, List<long>? excludedIds = null)
    {
        var serializedObject = await NetworkPacketWriterSerializer.SerializeAsync(writer);
        
        foreach (var roomUser in _users
                     .Values
                     .Where(x => excludedIds == null || !excludedIds.Contains(x.Player.Player.Id)))
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
        try
        {
            var users = _users.Values;

            if (users.Count == 0)
            {
                return;
            }
            
            foreach (var roomUser in users)
            {
                await roomUser.RunPeriodicCheckAsync();
            }

            var bots = users
                .First()
                .Room
                .BotRepository
                .GetAll();

            if (bots.Count != 0)
            {
                await BroadcastDataAsync(new RoomBotStatusWriter
                {
                    Bots = bots
                });

                await BroadcastDataAsync(new RoomBotDataWriter
                {
                    Bots = bots
                });
            }

            await SendUserStatusUpdatesAsync();
            await SendUserDataUpdatesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
        }
    }

    public async Task SendUserStatusUpdatesAsync()
    {
        await BroadcastDataAsync(
            new RoomUserStatusWriter
            {
                Users = _users
                    .Values
            });
    }

    public async Task SendUserDataUpdatesAsync()
    {
        await BroadcastDataAsync(
            new RoomUserDataWriter
            {
                Users = _users
                    .Values
            });
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