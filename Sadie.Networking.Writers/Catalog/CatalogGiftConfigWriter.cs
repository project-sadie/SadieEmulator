using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogGiftConfigWriter : AbstractPacketWriter
{
    public CatalogGiftConfigWriter()
    {
        WriteShort(ServerPacketId.CatalogGiftConfig);
        WriteBool(true);
        WriteInteger(3); // special price?
        WriteInteger(0); // giftWrappers count
        WriteInteger(0); // BOX_TYPES count
        WriteInteger(0); // RIBBON_TYPES count
        WriteInteger(0); // giftFurnis count
    }
}