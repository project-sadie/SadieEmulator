using System.Collections.Concurrent;
using AutoMapper;
using DotNetty.Transport.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.Db;
using Sadie.Db.Models.Players;

namespace Sadie.Networking.Client;

public class NetworkClientRepository(
    ILogger<NetworkClientRepository> logger,
    IPlayerRepository playerRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IPlayerHelperService playerHelperService,
    IMapper mapper) : INetworkClientRepository
{
    private readonly ConcurrentDictionary<IChannelId, INetworkClient> _clients = new();
    private readonly ConcurrentDictionary<string, byte> _removalGuard = new();
    
    public void AddClient(IChannelId channelId, INetworkClient client)
    {
        _clients[channelId] = client;
    }

    public async Task<bool> TryRemoveAsync(IChannelId channelId)
    {
        if (!_removalGuard.TryAdd(channelId.AsShortText(), 0))
        {
            return false;
        }

        if (!_clients.TryRemove(channelId, out var client))
        {
            return false;
        }

        var player = client.Player;
        var roomUser = client.RoomUser;

        if (roomUser != null)
        {
            await roomUser.Room.UserRepository.TryRemoveAsync(roomUser.Player.Player.Id, true, true);
        }

        try
        {
            if (player != null)
            {
                if (!await playerRepository.TryRemovePlayerAsync(player.Player.Id))
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

                var playerDataEntity = mapper.Map<PlayerData>(player.Player.Data);

                await using var dbContext = await dbContextFactory.CreateDbContextAsync();
                dbContext.Entry(playerDataEntity).Property(x => x.IsOnline).IsModified = true;
                await dbContext.SaveChangesAsync();
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another thread removed the player already, safe to ignore
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