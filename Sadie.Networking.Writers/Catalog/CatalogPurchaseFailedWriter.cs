using Sadie.Game.Catalog;
using Sadie.Game.Navigator;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogPurchaseFailedWriter : NetworkPacketWriter
{
    public CatalogPurchaseFailedWriter(CatalogPurchaseError error)
    {
        WriteShort(ServerPacketId.CatalogPurchaseFailed);
        WriteInteger((int) error);
    }
}