using System.Collections.Concurrent;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Game.Players;

namespace Sadie.Networking.Client;

public class NetworkClientRepository(
    ILogger<NetworkClientRepository> logger,
    PlayerRepository playerRepository,
    SadieContext dbContext) : INetworkClientRepository
{
    private readonly ConcurrentDictionary<IChannelId, INetworkClient> _clients = new();

    public void AddClient(IChannelId channelId, INetworkClient client)
    {
        _clients[channelId] = client;
    }

    public async Task<bool> TryRemoveAsync(IChannelId channelId)
    {
        if (!_clients.ContainsKey(channelId))
        {
            return true;
        }
        
        try
        {
            var result = _clients.TryRemove(channelId, out var client);
            
            if (!result)
            {
                logger.LogError("Failed to remove a network client.");
                return false;
            }

            var player = client.Player;
            var roomUser = client.RoomUser;
            
            await client.DisposeAsync();

            if (player != null)
            {
                await PlayerHelpersToClean.UpdatePlayerStatusForFriendsAsync(
                    player, 
                    player.GetMergedFriendships(), 
                    false, 
                    false, 
                    playerRepository);
                
                player.Data.IsOnline = false;
                // await dbContext.Database.ExecuteSqlRawAsync($"UPDATE `player_data` SET `is_online` = @online WHERE `player_id` = @playerId", 0, player.Id);
                
                if (!await playerRepository.TryRemovePlayerAsync(client.Player.Id))
                {
                    logger.LogError("Failed to remove player whilst disposing network client.");
                    return false;
                }
            }

            if (roomUser != null)
            {
                await roomUser.Room.UserRepository.TryRemoveAsync(roomUser.Id, true, true);
            }

            return result;
            
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to remove network client: {e}");
            return false;
        }
    }

    public async Task DisconnectIdleClientsAsync()
    {
        var idleClients = _clients.Values
            .Where(x => (DateTime.Now - x.LastPing).TotalSeconds >= 60)
            .Take(50)
            .ToList();

        if (idleClients.Count < 1)
        {
            return;
        }
        
        logger.LogWarning($"Disconnecting {idleClients.Count} idle players");

        foreach (var client in idleClients)
        {
            if (!await TryRemoveAsync(client.Channel.Id))
            {
                logger.LogError("Failed to dispose of network client");
            }
        }
    }

    public INetworkClient? TryGetClientByChannelId(IChannelId channelId)
    {
        return _clients.GetValueOrDefault(channelId);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var client in _clients.Keys)
        {
            if (!await TryRemoveAsync(client))
            {
                logger.LogError("Failed to dispose of network client");
            }
        }
    }
}