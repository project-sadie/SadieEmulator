using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Networking.Client;

public class NetworkClientFactory(IServiceProvider serviceProvider) : INetworkClientFactory
{
    public INetworkClient CreateClient(IChannel channel)
    {
        return ActivatorUtilities.CreateInstance<NetworkClient>(serviceProvider, channel);
    }
}