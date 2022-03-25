using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;

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
}