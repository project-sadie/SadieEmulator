using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerId.CatalogMode)]
public class CatalogModeEventHandler() : INetworkPacketEventHandler
{
    public string? Mode { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (string.IsNullOrWhiteSpace(Mode))
        {
            return;
        }
        
        client.Player!.State.CatalogMode = Mode;
        
        await client.WriteToStreamAsync(new CatalogModeWriter
        {
            Mode = Mode == "BUILDERS_CLUB" ? 1 : 0
        });
    }
}