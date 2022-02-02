using Sadie.Networking.Client;

namespace Sadie.Networking.Packets.Client.Handshake
{
    public class ClientVersionEvent : INetworkPacketEvent
    {
        public Task HandleAsync(INetworkClient client, INetworkPacketReader reader) => Task.CompletedTask;
    }
}