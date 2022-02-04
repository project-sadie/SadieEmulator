using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Handshake;

namespace Sadie.Networking.Packets.Client.Handshake
{
    public class MachineIdEvent : INetworkPacketEvent
    {
        public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
        {
            var machineId = reader.ReadString();
            var fingerprint = reader.ReadString();
            var unknown = reader.ReadString();

            await client.WriteToStreamAsync(new MachineIdWriter(fingerprint).GetAllBytes());
        }
    }
}