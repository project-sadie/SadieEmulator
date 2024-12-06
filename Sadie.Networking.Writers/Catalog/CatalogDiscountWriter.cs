using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogDiscount)]
public class CatalogDiscountWriter : AbstractPacketWriter
{
    public required int Unknown1 { get; init; }
    public required int Unknown2 { get; init; }
    public required int Unknown3 { get; init; }
    public required int Unknown4 { get; init; }
    public required int Unknown5 { get; init; }
}