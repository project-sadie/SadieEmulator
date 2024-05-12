using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Handshake;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Handshake;

namespace Sadie.Networking.Events.Handlers.Handshake;

public class UniqueIdEventHandler(UniqueIdEventParser eventParser) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.UniqueId;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);
        
        await client.WriteToStreamAsync(new UniqueIdWriter
        {
            MachineId = eventParser.Fingerprint
        });
    }
}