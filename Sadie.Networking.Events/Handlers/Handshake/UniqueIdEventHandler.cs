using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Handshake;

namespace Sadie.Networking.Events.Handlers.Handshake;

[PacketId(EventHandlerIds.UniqueId)]
public class UniqueIdEventHandler : INetworkPacketEventHandler
{
    public required string Fingerprint { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new UniqueIdWriter
        {
            MachineId = Fingerprint
        });
    }
}