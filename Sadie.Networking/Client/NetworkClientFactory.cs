using System.Net.Sockets;
using System.Net.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Networking.WebSockets;

namespace Sadie.Networking.Client;

public class NetworkClientFactory : INetworkClientFactory
{
    private readonly IServiceProvider _serviceProvider;

    public NetworkClientFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public INetworkClient CreateClient(Guid guid, TcpClient tcpClient)
    {
        return ActivatorUtilities.CreateInstance<NetworkClient>(_serviceProvider, guid, tcpClient);
    }

    public INetworkClient CreateClient(Guid guid, WebSocket tcpClient)
    {
        return ActivatorUtilities.CreateInstance<WsNetworkNetworkClient>(_serviceProvider, guid, tcpClient);
    }
}