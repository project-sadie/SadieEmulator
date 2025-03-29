using System.Collections.Concurrent;
using DotNetty.Transport.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sadie.API.Game.Players;
using Sadie.Database;

namespace Sadie.Networking.Client;

public class NetworkClientRepository(
    ILogger<NetworkClientRepository> logger,
    IPlayerRepository playerRepository,
    IDbContextFactory<SadieContext> dbContextFactory,
    IPlayerHelperService playerHelperService) : INetworkClientRepository
{
    private readonly ConcurrentDictionary<IChannelId, INetworkClient> _clients = new();

    public void AddClient(IChannelId channelId, INetworkClient client)
    {
        _clients[channelId] = client;
    }

    public async Task<bool> TryRemoveAsync(IChannelId channelId)
    {
        if (!_clients.TryRemove(channelId, out var client))
        {
            logger.LogError("Failed to remove a network client.");
            return false;
        }

        var player = client.Player;
        var roomUser = client.RoomUser;

        if (roomUser != null)
        {
            await roomUser.Room.UserRepository.TryRemoveAsync(roomUser.Id, true, true);
        }
        
        if (player != null)
        {
            if (!await playerRepository.TryRemovePlayerAsync(player.Id))
            {
                logger.LogError("Failed to remove player whilst disposing network client.");
                return false;
            }
                
            await playerHelperService.UpdatePlayerStatusForFriendsAsync(
                player, 
                player.GetMergedFriendships(), 
                false, 
                false, 
                playerRepository);
                
            player.Data.IsOnline = false;

            await using var dbContext = await dbContextFactory.CreateDbContextAsync();
            dbContext.Entry(player.Data).Property(x => x.IsOnline).IsModified = true;
            await dbContext.SaveChangesAsync();
        }
        
        await client.DisposeAsync();
        return true;
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