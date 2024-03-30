using Sadie.Game.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Catalog;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

public class CatalogPageEvent(CatalogPageParser parser, CatalogPageRepository catalogPageRepo) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);
        
        var (exists, page) = catalogPageRepo.TryGet(parser.PageId);

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
            parser.CatalogMode).GetAllBytes());
    }
}