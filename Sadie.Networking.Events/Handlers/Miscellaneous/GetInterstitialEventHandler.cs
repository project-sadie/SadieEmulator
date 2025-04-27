using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Miscellaneous;

[PacketId(EventHandlerId.GetInterstitial)]
public class GetInterstitialEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client)
    {
        return Task.CompletedTask;
    }
}