using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Unsorted;

[PacketId(EventHandlerId.GetBadgePointLimits)]
public class GetBadgePointLimitsEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
    }
}