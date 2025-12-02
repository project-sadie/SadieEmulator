using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Packets.Writers.Handshake;

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