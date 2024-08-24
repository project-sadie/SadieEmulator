using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogGiftConfig)]
public class CatalogGiftConfigWriter : AbstractPacketWriter
{
    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteBool(true);
        writer.WriteInteger(3); // special price?
        writer.WriteInteger(0); // giftWrappers count
        writer.WriteInteger(0); // BOX_TYPES count
        writer.WriteInteger(0); // RIBBON_TYPES count
        writer.WriteInteger(0); // giftFurnis count
    }
}