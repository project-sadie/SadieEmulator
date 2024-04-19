using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Unknown;

public class GetTargetedOfferEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.GetTargetedOffer;

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        return Task.CompletedTask;
    }
}