using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Networking.Writers.Catalog;
using Sadie.Shared.Attributes;

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