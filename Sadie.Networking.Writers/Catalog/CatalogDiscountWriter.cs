using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogDiscount)]
public class CatalogDiscountWriter : AbstractPacketWriter
{
    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
    }
}