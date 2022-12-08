using Sadie.Game.Catalog;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogPageWriter : NetworkPacketWriter
{
    public CatalogPageWriter(CatalogPage page, string catalogMode)
    {
        WriteShort(ServerPacketId.CatalogPage);
        WriteInteger(page.Id);
        WriteString(catalogMode);
        
        // TODO: WRITE CATALOG PAGE
        
        WriteInteger(page.Items.Count);

        foreach (var item in page.Items)
        {
            // TODO: WRITE CATALOG ITEM
        }
        
        WriteInteger(0);
        WriteBool(false);
        
        // TODO: serialize extra?
    }
}