using Sadie.Networking.Client;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Unsorted;

[PacketId(EventHandlerId.GetTargetedOffer)]
public class GetTargetedOfferEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client)
    {
        return Task.CompletedTask;
    }
}