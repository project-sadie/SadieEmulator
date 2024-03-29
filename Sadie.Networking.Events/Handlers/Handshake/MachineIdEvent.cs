using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Handshake;

namespace Sadie.Networking.Events.Handlers.Handshake;

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