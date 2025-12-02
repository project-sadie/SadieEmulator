using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Catalog;

[PacketId(ServerPacketId.CatalogPurchaseFailed)]
public class CatalogPurchaseFailedWriter : AbstractPacketWriter
{
    public required int Error { get; init; }
}