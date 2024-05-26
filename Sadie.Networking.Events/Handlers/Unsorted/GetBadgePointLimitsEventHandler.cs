using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Unsorted;

public class GetBadgePointLimitsEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.GetBadgePointLimits;

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        return Task.CompletedTask;
    }
}