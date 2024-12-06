using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerId.CatalogDiscount)]
public class CatalogDiscountEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new CatalogDiscountWriter
        {
            Unknown1 = 0,
            Unknown2 = 0,
            Unknown3 = 0,
            Unknown4 = 0,
            Unknown5 = 0
        });
    }
}