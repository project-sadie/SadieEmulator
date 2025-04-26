using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerId.CatalogDiscount)]
public class CatalogDiscountEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new CatalogBundleDiscountRuleSetWriter
        {
            MaxPurchaseSize = 0,
            BundleSize = 0,
            BundleDiscountSize = 0,
            BonusThreshold = 0,
            AdditionalBonusDiscountThresholdQuantities = 0
        });
    }
}