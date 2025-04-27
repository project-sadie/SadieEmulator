using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Miscellaneous;

[PacketId(EventHandlerId.GetBadgePointLimits)]
public class GetBadgePointLimitsEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client)
    {
        return Task.CompletedTask;
    }
}