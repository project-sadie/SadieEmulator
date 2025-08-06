using Sadie.Networking.Client;
using Sadie.Networking.Writers.Catalog;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerId.CatalogGiftConfig)]
public class CatalogGiftConfigEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new CatalogGiftWrappingConfigWriter
        {
            Enabled = true,
            Price = 3,
            GiftWrappers = [],
            BoxTypes = [],
            RibbonTypes = [],
            GiftFurniture = [],
        });
    }
}