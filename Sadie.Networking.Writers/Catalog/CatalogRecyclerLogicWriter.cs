using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogRecyclerLogic)]
public class CatalogRecyclerLogicWriter : AbstractPacketWriter
{
    public required int PrizeSize { get; init; }
}