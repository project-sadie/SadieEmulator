using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Sadie.Networking.Client;

public class NetworkClientRepository : INetworkClientRepository
{
    private readonly ILogger<NetworkClientRepository> _logger;
    private readonly ConcurrentDictionary<Guid, INetworkClient> _clients;

    public NetworkClientRepository(ILogger<NetworkClientRepository> logger)
    {
        _logger = logger;
        _clients = new ConcurrentDictionary<Guid, INetworkClient>();
    }

    public void AddClient(Guid guid, INetworkClient client)
    {
        _clients[guid] = client;
    }

    public bool TryRemove(Guid guid)
    {
        return _clients.TryRemove(guid, out _);
    }

    public async Task DisconnectIdleClientsAsync()
    {
        var idleClients = _clients.Values
            .Where(x => x.LastPing != default && (DateTime.Now - x.LastPing).TotalSeconds >= 60)
            .Take(50)
            .ToList();

        if (idleClients.Count < 1)
        {
            return;
        }
        
        _logger.LogWarning($"Disconnecting {idleClients.Count} idle players");

        foreach (var client in idleClients)
        {
            await client.DisposeAsync();
        }
    }
}