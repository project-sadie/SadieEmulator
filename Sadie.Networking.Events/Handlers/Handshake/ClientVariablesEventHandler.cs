using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Handshake;

[PacketId(EventHandlerIds.ClientVariables)]
public class ClientVariablesEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader) => Task.CompletedTask;
}