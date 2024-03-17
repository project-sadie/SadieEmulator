using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogPurchaseFailedWriter : NetworkPacketWriter
{
    public CatalogPurchaseFailedWriter(int error)
    {
        WriteShort(ServerPacketId.CatalogPurchaseFailed);
        WriteInteger(error);
    }
}