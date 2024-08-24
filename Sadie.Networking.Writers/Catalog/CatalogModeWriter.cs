using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogMode)]
public class CatalogModeWriter : AbstractPacketWriter
{
    public required int Mode { get; init; }
}