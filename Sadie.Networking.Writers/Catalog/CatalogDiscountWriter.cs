using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogDiscountWriter : NetworkPacketWriter
{
    public CatalogDiscountWriter()
    {
        WriteShort(ServerPacketId.CatalogDiscount);
        WriteInteger(0);
        WriteInteger(0);
        WriteInteger(0);
        WriteInteger(0);
        WriteInteger(0); // TODO: actual structure
    }   
}