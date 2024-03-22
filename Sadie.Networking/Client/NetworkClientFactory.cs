using System.Net.Sockets;
using Fleck;
using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Networking.Client;

public class NetworkClientFactory(IServiceProvider serviceProvider) : INetworkClientFactory
{
    public INetworkClient CreateClient(Guid guid, IWebSocketConnection tcpClient)
    {
        return ActivatorUtilities.CreateInstance<NetworkClient>(serviceProvider, guid, tcpClient);
    }
}