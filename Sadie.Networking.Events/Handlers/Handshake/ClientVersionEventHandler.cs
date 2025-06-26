using Sadie.Networking.Client;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Handshake;

[PacketId(EventHandlerId.ClientVersion)]
public class ClientVersionEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client) => Task.CompletedTask;
}