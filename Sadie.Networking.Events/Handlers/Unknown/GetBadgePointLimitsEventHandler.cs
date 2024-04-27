using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Unknown;

public class GetBadgePointLimitsEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.GetBadgePointLimits;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
    }
}