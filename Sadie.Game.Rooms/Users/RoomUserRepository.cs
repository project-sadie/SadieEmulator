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
    public bool TryAdd(IRoomUser user) => _users.TryAdd(user.Id, user);
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
        if (notifyLeft)
        {
            var writer = new RoomUserLeftWriter
            {
                UserId = id.ToString()
            };

            await BroadcastDataAsync(writer, [id]);
        }

        var result = _users.TryRemove(id, out var roomUser);

        if (!result || roomUser == null)
        {
            logger.LogError($"Failed to remove a room user");
            return;
        }

        var player = roomUser.Player;
        
        player.State.CurrentRoomId = 0;
        
        await playerHelperService.UpdatePlayerStatusForFriendsAsync(
            player, 
            player.GetMergedFriendships(),
            player.Data.IsOnline, 
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
        var serializedObject = NetworkPacketWriterSerializer.Serialize(writer);
        
        foreach (var roomUser in _users
                     .Values
                     .Where(x => excludedIds == null || !excludedIds.Contains(x.Id)))
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
            await Parallel.ForEachAsync(_users.Values, async (user, ct) =>
            {
                await user.RunPeriodicCheckAsync();
            });

            var users = _users
                .Values;

            if (!_users.IsEmpty)
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
            
            var statusWriter = new RoomUserStatusWriter
            {
                Users = users
                    .Where(x => x.NeedsStatusUpdate)
                    .ToList()
            };

            var dataWriter = new RoomUserDataWriter
            {
                Users = users
            };

            await BroadcastDataAsync(statusWriter);
            await BroadcastDataAsync(dataWriter);
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
            await TryRemoveAsync(user.Id, false);
        }
        
        _users.Clear();
    }
}