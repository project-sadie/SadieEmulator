using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Packets.Writers.Catalog;

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