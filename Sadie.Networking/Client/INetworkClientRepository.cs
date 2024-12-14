using DotNetty.Transport.Channels;
using Sadie.API.Networking;

namespace Sadie.Networking.Client;

public interface INetworkClientRepository : IAsyncDisposable
{
    void AddClient(IChannelId channelId, INetworkClient client);
    Task<bool> TryRemoveAsync(IChannelId channelId);
    Task DisconnectIdleClientsAsync();
    INetworkClient? TryGetClientByChannelId(IChannelId channelId);
    Task BroadcastDataAsync(AbstractPacketWriter writer);
}