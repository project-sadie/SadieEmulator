using System.Collections.Concurrent;

namespace Sadie.Networking.Client
{
    public class NetworkClientRepository : INetworkClientRepository
    {
        private readonly ConcurrentDictionary<int, INetworkClient> _clients;

        public NetworkClientRepository()
        {
            _clients = new ConcurrentDictionary<int, INetworkClient>();
        }

        public void AddClient(INetworkClient client)
        {
            _clients[_clients.Count] = client;
        }

        public bool TryRemove(int clientId)
        {
            return _clients.TryRemove(clientId, out var _);
        }
    }
}