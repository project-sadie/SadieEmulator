using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogPurchaseFailedWriter : AbstractPacketWriter
{
    public CatalogPurchaseFailedWriter(int error)
    {
        WriteShort(ServerPacketId.CatalogPurchaseFailed);
        WriteInteger(error);
    }
}