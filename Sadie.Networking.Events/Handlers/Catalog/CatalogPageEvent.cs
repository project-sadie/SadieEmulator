using Sadie.Game.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

public class CatalogPageEvent(CatalogPageRepository catalogPageRepo) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var pageId = reader.ReadInteger();
        var unknown1 = reader.ReadInteger();
        var catalogMode = reader.ReadString();
        
        var (exists, page) = catalogPageRepo.TryGet(pageId);

        if (!exists || page is not { Enabled: true } || !page.Visible)
        {
            return;
        }

        await client.WriteToStreamAsync(new CatalogPageWriter(
            page.Id,
            page.Layout, 
            page.HeaderImage,
            page.TeaserImage, 
            page.SpecialImage, 
            page.PrimaryText, 
            page.SecondaryText, 
            page.TeaserText, 
            page.Items, 
            catalogMode).GetAllBytes());
    }
}