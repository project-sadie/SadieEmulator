using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Unknown;

[PacketId(EventHandlerIds.GetBadgePointLimits)]
public class GetBadgePointLimitsEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
    }
}