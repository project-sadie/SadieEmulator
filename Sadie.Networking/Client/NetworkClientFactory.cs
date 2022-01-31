using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Client
{
    public class NetworkClientFactory : INetworkClientFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public NetworkClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public INetworkClient CreateClient(TcpClient tcpClient)
        {
            return ActivatorUtilities.CreateInstance<NetworkClient>(_serviceProvider, tcpClient, _serviceProvider.GetRequiredService<INetworkPacketHandler>());
        }
    }
}