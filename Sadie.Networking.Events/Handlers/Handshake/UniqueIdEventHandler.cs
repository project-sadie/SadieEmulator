using Sadie.Networking.Client;
using Sadie.Networking.Writers.Handshake;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Handshake;

[PacketId(EventHandlerId.UniqueId)]
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