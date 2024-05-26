using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerIds.CatalogMarketplaceConfig)]
public class CatalogMarketplaceConfigEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
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