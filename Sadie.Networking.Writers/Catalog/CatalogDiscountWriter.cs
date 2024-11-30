using Sadie.API;
using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogDiscount)]
public class CatalogDiscountWriter : AbstractPacketWriter
{
    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
    }
}