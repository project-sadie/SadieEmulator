using System.Net;
using System.Net.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Interfaces.Networking.Client;

namespace Sadie.Networking.Client;

public class NetworkClientFactory(IServiceProvider serviceProvider) : INetworkClientFactory
{
    public INetworkClient CreateClient(IPAddress ipAddress, Guid guid, WebSocket webSocket)
    {
        return ActivatorUtilities.CreateInstance<NetworkClient>(serviceProvider, ipAddress, guid, webSocket);
    }
}