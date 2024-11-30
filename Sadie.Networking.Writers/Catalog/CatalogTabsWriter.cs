using Sadie.API;
using Sadie.API.Networking;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogPages)]
public class CatalogTabsWriter : AbstractPacketWriter
{
    public required string? Mode { get; init; }
    public required List<CatalogPage> TabPages { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteBool(true);
        writer.WriteInteger(0);
        writer.WriteInteger(-1);
        writer.WriteString("root");
        writer.WriteString("");
        writer.WriteInteger(0);
        writer.WriteInteger(TabPages.Count);

        foreach (var page in TabPages)
        {
            AppendPage(page, writer);
        }
        
        writer.WriteBool(false);
        writer.WriteString(Mode);
    }

    private void AppendPage(CatalogPage page, INetworkPacketWriter writer)
    {
        writer.WriteBool(page.Visible);
        writer.WriteInteger(page.IconId);
        writer.WriteInteger(page.Enabled ? page.Id : -1);
        writer.WriteString(page.Name);
        writer.WriteString(page.Caption);
        writer.WriteInteger(0);
        writer.WriteInteger(page.Pages.Count);
        
        foreach (var childPage in page.Pages)
        {
            AppendPage(childPage, writer);
        }
    }
}