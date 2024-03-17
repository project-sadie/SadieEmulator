using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Sadie.Networking.Client;

public class NetworkClientRepository(ILogger<NetworkClientRepository> logger) : INetworkClientRepository
{
    private readonly ConcurrentDictionary<Guid, INetworkClient> _clients = new();

    public void AddClient(Guid guid, INetworkClient client)
    {
        _clients[guid] = client;
    }

    public async Task<bool> TryRemoveAsync(Guid guid)
    {
        try
        {
            var result = _clients.TryRemove(guid, out var client);

            if (client != null)
            {
                await client.DisposeAsync();
            }

            return result;
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to remove network client: {e.Message}");
            return false;
        }
    }

    public async Task DisconnectIdleClientsAsync()
    {
        var idleClients = _clients.Values
            .Where(x => x.LastPing != default && (DateTime.Now - x.LastPing).TotalSeconds >= 40)
            .Take(50)
            .ToList();

        if (idleClients.Count < 1)
        {
            return;
        }
        
        logger.LogWarning($"Disconnecting {idleClients.Count} idle players");

        foreach (var client in idleClients)
        {
            if (!await TryRemoveAsync(client.Guid))
            {
                logger.LogError("Failed to dispose of network client");
            }
        }
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