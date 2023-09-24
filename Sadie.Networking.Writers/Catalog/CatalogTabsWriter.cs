using Sadie.Game.Catalog;
using Sadie.Game.Catalog.Pages;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogTabsWriter : NetworkPacketWriter
{
    public CatalogTabsWriter(string mode, List<CatalogPage> tabPages)
    {
        WriteShort(ServerPacketId.CatalogPages);
        WriteBool(true);
        WriteInteger(0);
        WriteInteger(-1);
        WriteString("root");
        WriteString("");
        WriteInteger(0);
        WriteInteger(tabPages.Count);

        foreach (var page in tabPages)
        {
            AppendPage(page);
        }
        
        WriteBool(true);
        WriteString(mode);
    }

    private void AppendPage(CatalogPage page)
    {
        WriteBool(page.Visible);
        WriteInteger(page.Icon);
        WriteInteger(page.Enabled ? page.Id : -1);
        WriteString(page.Caption);
        WriteString(page.Name);
        WriteInteger(0); // TODO: offer id count
        WriteInteger(page.Pages.Count);
        
        foreach (var childPage in page.Pages)
        {
            AppendPage(childPage);
        }
    }
}