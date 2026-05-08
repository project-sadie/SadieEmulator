using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogRecyclerLogic)]
public class CatalogRecyclerLogicWriter : AbstractPacketWriter
{
    public required int PrizeSize { get; init; }
}