using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogPurchaseUnavailable)]
public class CatalogPurchaseUnavailableWriter : AbstractPacketWriter
{
    public int Code { get; init; }
}