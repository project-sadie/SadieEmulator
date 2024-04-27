using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogDiscountWriter : AbstractPacketWriter
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