using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerId.GetTargetedOffer)]
public class GetTargetedOfferEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client)
    {
        return Task.CompletedTask;
    }
}