using System.Collections.Concurrent;

namespace Sadie.Networking.Client;

public class NetworkClientRepository : INetworkClientRepository
{
    private readonly ConcurrentDictionary<Guid, INetworkClient> _clients;

    public NetworkClientRepository()
    {
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
}