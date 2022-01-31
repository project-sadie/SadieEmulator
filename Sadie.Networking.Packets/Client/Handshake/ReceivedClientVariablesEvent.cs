using Sadie.Networking.Client;

namespace Sadie.Networking.Packets.Client.Handshake
{
    public class ReceivedClientVariablesEvent : INetworkPacketEvent
    {
        public Task HandleAsync(INetworkClient client, INetworkPacketReader reader) => Task.CompletedTask;
    }
}