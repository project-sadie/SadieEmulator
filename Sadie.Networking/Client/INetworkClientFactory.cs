using DotNetty.Transport.Channels;

namespace Sadie.Networking.Client;

public interface INetworkClientFactory
{
    INetworkClient CreateClient(IChannel channel);
}