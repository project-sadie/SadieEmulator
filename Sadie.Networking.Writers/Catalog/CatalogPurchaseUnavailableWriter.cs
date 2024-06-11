using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogPurchaseUnavailable)]
public class CatalogPurchaseUnavailableWriter : AbstractPacketWriter
{
    public int Code { get; init; }
}