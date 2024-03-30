using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Handshake;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Handshake;

namespace Sadie.Networking.Events.Handlers.Handshake;

public class UniqueIdEvent(UniqueIdParser parser) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);
        await client.WriteToStreamAsync(new UniqueIdWriter(parser.Fingerprint).GetAllBytes());
    }
}