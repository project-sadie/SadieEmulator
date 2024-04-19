using System.Collections.Concurrent;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;

namespace Sadie.Networking.Client;

public class NetworkClientRepository(ILogger<NetworkClientRepository> logger) : INetworkClientRepository
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

            if (client != null)
            {
                await client.DisposeAsync();
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
            .Where(x => x.LastPing != default && (DateTime.Now - x.LastPing).TotalSeconds >= 60)
            .Take(100)
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