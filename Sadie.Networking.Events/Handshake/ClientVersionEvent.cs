using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handshake;

public class ClientVersionEvent : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader) => Task.CompletedTask;
}