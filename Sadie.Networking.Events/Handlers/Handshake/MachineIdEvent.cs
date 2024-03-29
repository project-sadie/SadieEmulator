using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Handshake;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Handshake;

namespace Sadie.Networking.Events.Handlers.Handshake;

public class MachineIdEvent(MachineIdParser parser) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new MachineIdWriter(parser.Fingerprint).GetAllBytes());
    }
}