using Sadie.Game.Catalog;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogPagesWriter : NetworkPacketWriter
{
    public CatalogPagesWriter(int mode, List<CatalogPage> pages)
    {
        WriteShort(ServerPacketId.CatalogPages);
        WriteBool(true);
        WriteInteger(0);
        WriteInteger(-1);
        WriteString("root");
        WriteString("");
        WriteInteger(0);
        WriteInteger(pages.Count);

        foreach (var page in pages)
        {
            AppendPage(page);
        }
        
        WriteBool(false);
        WriteInteger(mode);
    }

    private void AppendPage(CatalogPage page)
    {
        WriteBool(page.Visible);
        WriteInteger(page.Icon);
        WriteInteger(page.Enabled ? page.Id : -1);
        WriteString(page.Name);
        WriteString(page.Caption);
        WriteInteger(0); // TODO: offer id count

        var childPages = page.Pages;
        
        WriteInteger(childPages.Count);

        foreach (var childPage in childPages)
        {
            AppendPage(childPage);
        }
    }
}