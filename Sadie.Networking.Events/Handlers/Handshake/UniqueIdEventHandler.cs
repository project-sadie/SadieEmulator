using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Handshake;

namespace Sadie.Networking.Events.Handlers.Handshake;

[PacketId(EventHandlerIds.UniqueId)]
public class UniqueIdEventHandler(UniqueIdEventParser eventParser) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);
        
        await client.WriteToStreamAsync(new UniqueIdWriter
        {
            MachineId = eventParser.Fingerprint
        });
    }
}