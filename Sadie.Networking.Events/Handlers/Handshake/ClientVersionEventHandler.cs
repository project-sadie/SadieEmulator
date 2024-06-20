using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Handshake;

[PacketId(EventHandlerIds.ClientVersion)]
public class ClientVersionEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client) => Task.CompletedTask;
}