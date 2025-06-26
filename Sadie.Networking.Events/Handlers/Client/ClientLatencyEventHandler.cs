using Sadie.Networking.Client;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Client;

[PacketId(EventHandlerId.ClientLatency)]
public class ClientLatencyEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
    }
}