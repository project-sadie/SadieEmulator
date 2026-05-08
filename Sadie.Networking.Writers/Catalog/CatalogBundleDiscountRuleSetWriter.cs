using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogDiscount)]
public class CatalogBundleDiscountRuleSetWriter : AbstractPacketWriter
{
    public required int MaxPurchaseSize { get; init; }
    public required int BundleSize { get; init; }
    public required int BundleDiscountSize { get; init; }
    public required int BonusThreshold { get; init; }
    public required int AdditionalBonusDiscountThresholdQuantities { get; init; }
}