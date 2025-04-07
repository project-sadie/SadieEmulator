using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms.Users;
using Sadie.API.Networking;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Packets.Writers.Users;
using Sadie.Networking.Serialization;

namespace Sadie.Game.Rooms.Users;

public class RoomUserRepository(ILogger<RoomUserRepository> logger,
    IPlayerRepository playerRepository,
    IPlayerHelperService playerHelperService) : IRoomUserRepository
{
    private readonly ConcurrentDictionary<long, IRoomUser> _users = new();

    public ICollection<IRoomUser> GetAll() => _users.Values;
    public bool TryAdd(IRoomUser user) => _users.TryAdd(user.Player.Id, user);
    public bool TryGetById(long id, out IRoomUser? user) => _users.TryGetValue(id, out user);

    public bool TryGetByUsername(string username, out IRoomUser? user)
    {
        user = _users.Values.FirstOrDefault(x => x.Player.Username == username);
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
        
        /*await playerHelperService.UpdatePlayerStatusForFriendsAsync(
            player, 
            player.GetMergedFriendships(),
            player.Data.IsOnline, 
            false,
            playerRepository);*/
        
        if (hotelView)
        {
            await roomUser.NetworkObject.WriteToStreamAsync(new RoomUserHotelViewWriter());
        }
        
        await roomUser.DisposeAsync();
    }
    
    public int Count => _users.Count;
    
    public async Task BroadcastDataAsync(AbstractPacketWriter writer, List<long>? excludedIds = null)
    {
        var serializedObject = NetworkPacketWriterSerializer.Serialize(writer);
        
        foreach (var roomUser in _users
                     .Values
                     .Where(x => excludedIds == null || !excludedIds.Contains(x.Player.Id)))
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

            foreach (var roomUser in users)
            {
                await roomUser.RunPeriodicCheckAsync();
            }

            if (users.Count != 0)
            {
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
                    .Where(x => x.NeedsStatusUpdate)
                    .ToList()
            });
    }

    public async Task SendUserDataUpdatesAsync()
    {
        await BroadcastDataAsync(
            new RoomUserDataWriter
            {
                Users = _users.Values
            });
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var user in _users.Values)
        {
            await TryRemoveAsync(user.Player.Id, false);
        }
        
        _users.Clear();
    }
}