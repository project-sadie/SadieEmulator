using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

public class CatalogMarketplaceConfigEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.CatalogMarketplaceConfig;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new CatalogMarketplaceConfigWriter(
            true, 
            1, 
            10, 
            5, 
            1, 
            1000000, 
            48, 
            7));
    }
}