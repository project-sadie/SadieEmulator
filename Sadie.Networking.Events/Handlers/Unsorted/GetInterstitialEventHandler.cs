using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Unsorted;

[PacketId(EventHandlerIds.GetInterstitial)]
public class GetInterstitialEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        return Task.CompletedTask;
    }
}