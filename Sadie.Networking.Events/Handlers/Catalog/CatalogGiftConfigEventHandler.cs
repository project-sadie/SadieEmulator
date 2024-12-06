using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerId.CatalogGiftConfig)]
public class CatalogGiftConfigEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new CatalogGiftConfigWriter
        {
            Unknown1 = true,
            Unknown2 = 3,
            Unknown3 = 0,
            Unknown4 = 0,
            Unknown5 = 0,
            Unknown6 = 0
        });
    }
}