using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Catalog;

[PacketId(ServerPacketId.CatalogMode)]
public class CatalogModeWriter : AbstractPacketWriter
{
    public required int Mode { get; init; }
}