using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Handshake;

namespace Sadie.Networking.Packets.Client.Handshake
{
    public class ReceivedMachineIdEvent : INetworkPacketEvent
    {
        public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
        {
            await client.WriteToStreamAsync(new SendUniqueMachineIdPacket(reader.ReadString()).GetAllBytes());
        }
    }
}