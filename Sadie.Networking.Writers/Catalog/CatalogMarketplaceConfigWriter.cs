using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogMarketplaceConfig)]
public class CatalogMarketplaceConfigWriter : AbstractPacketWriter
{
    public required bool Unknown { get; init; }
    public required int CommissionPercent { get; init; }
    public required int Credits { get; init; }
    public required int Advertisements { get; init; }
    public required int MinPrice { get; init; }
    public required int MaxPrice { get; init; }
    public required int HoursInMarketplace { get; init; }
    public required int DaysToDisplay { get; init; }
}