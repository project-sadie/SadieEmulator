using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerId.CatalogMarketplaceConfig)]
public class CatalogMarketplaceConfigEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new CatalogMarketplaceConfigWriter
        {
            Unknown = true,
            CommissionPercent = 1,
            Credits = 10,
            Advertisements = 5,
            MinPrice = 1,
            MaxPrice = 1000000,
            HoursInMarketplace = 48,
            DaysToDisplay = 7
        });
    }
}