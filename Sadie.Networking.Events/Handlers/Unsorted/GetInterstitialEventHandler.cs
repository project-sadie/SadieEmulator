using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Unsorted;

[PacketId(EventHandlerId.GetInterstitial)]
public class GetInterstitialEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client)
    {
        return Task.CompletedTask;
    }
}