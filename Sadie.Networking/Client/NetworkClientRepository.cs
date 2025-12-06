using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.Db;

namespace Sadie.Networking.Client;

public class NetworkClientRepository(
    ILogger<NetworkClientRepository> logger,
    IPlayerRepository playerRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IPlayerHelperService playerHelperService,
    IMapper mapper) : INetworkClientRepository
{
    private readonly ConcurrentDictionary<Guid, INetworkClient> _clients = new();
    private readonly ConcurrentDictionary<Guid, byte> _removalGuard = new();

    public ICollection<INetworkClient> Clients => _clients.Values;
    
    public void AddClient(Guid guid, INetworkClient client)
    {
        _clients[guid] = client;
    }

    public async Task<bool> TryRemoveAsync(Guid guid)
    {
        if (!_removalGuard.TryAdd(guid, 0))
        {
            return false;
        }

        if (!_clients.TryRemove(guid, out var client))
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

                var friendships = player.GetMergedFriendships();

                if (friendships.Count != 0)
                {
                    await playerHelperService.UpdatePlayerStatusForFriendsAsync(
                        player,
                        friendships,
                        false,
                        false,
                        playerRepository);
                }
                
                await using var dbContext = await dbContextFactory.CreateDbContextAsync();
                
                await dbContext.Database
                    .ExecuteSqlRawAsync(
                        "UPDATE player_data SET is_online = 0 WHERE id = @p0 LIMIT 1", 
                        player.Player.Id);
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another thread removed the player already, safe to ignore
        }
        finally
        {
            _removalGuard.TryRemove(guid, out _);
        }
        
        await client.DisposeAsync();
        return true;
    }

    public async Task DisconnectIdleClientsAsync()
    {
        var idleClients = _clients.Values
            .Where(x => (DateTime.Now - x.LastPong).TotalSeconds >= 60)
            .Take(20)
            .ToList();
        
        if (idleClients.Count < 1)
        {
            return;
        }
        
        logger.LogWarning($"Disconnecting {idleClients.Count} idle players");
        
        var throttler = new SemaphoreSlim(10);

        var tasks = idleClients.Select(async client =>
        {
            await throttler.WaitAsync();

            try
            {
                await TryRemoveAsync(client.Guid);
            }
            finally
            {
                throttler.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

    public INetworkClient? TryGetClientByGuid(Guid guid)
    {
        return _clients.GetValueOrDefault(guid);
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