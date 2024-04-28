using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogRecyclerLogic)]
public class CatalogRecyclerLogicWriter : AbstractPacketWriter
{
    public required int PrizeSize { get; init; }
}