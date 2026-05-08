using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogGiftConfig)]
public class CatalogGiftWrappingConfigWriter : AbstractPacketWriter
{
    public required bool Enabled { get; init; }
    public required int Price { get; init; }
    public required List<int> GiftWrappers { get; init; }
    public required List<int> BoxTypes { get; init; }
    public required List<int> RibbonTypes { get; init; }
    public required List<int> GiftFurniture { get; init; }
}