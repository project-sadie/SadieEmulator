using System.Net.Sockets;
using System.Net.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Networking.WebSockets;

namespace Sadie.Networking.Client;

public class NetworkClientFactory(IServiceProvider serviceProvider) : INetworkClientFactory
{
    public INetworkClient CreateClient(Guid guid, TcpClient tcpClient)
    {
        return ActivatorUtilities.CreateInstance<NetworkClient>(serviceProvider, guid, tcpClient);
    }

    public INetworkClient CreateClient(Guid guid, WebSocket tcpClient)
    {
        return ActivatorUtilities.CreateInstance<WsNetworkNetworkClient>(serviceProvider, guid, tcpClient);
    }
}