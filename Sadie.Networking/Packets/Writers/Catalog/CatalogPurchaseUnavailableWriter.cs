using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Catalog;

[PacketId(ServerPacketId.CatalogPurchaseUnavailable)]
public class CatalogPurchaseUnavailableWriter : AbstractPacketWriter
{
    public int Code { get; init; }
}