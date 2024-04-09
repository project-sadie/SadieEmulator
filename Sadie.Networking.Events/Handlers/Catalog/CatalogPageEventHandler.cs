using Sadie.Game.Catalog.FrontPage;
using Sadie.Game.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Catalog;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

public class CatalogPageEventHandler(CatalogPageEventParser eventParser, 
    CatalogPageRepository catalogPageRepo,
    CatalogFrontPageItemRepository frontPageItemRepo) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.CatalogPage;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);
        
        var (exists, page) = catalogPageRepo.TryGet(eventParser.PageId);

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
            page.PrimaryText ?? string.Empty, 
            page.SecondaryText ?? string.Empty, 
            page.TeaserText ?? string.Empty,
            page.DetailsText ?? string.Empty,
            page.Items, 
            eventParser.CatalogMode,
            false,
            frontPageItemRepo.Items).GetAllBytes());
    }
}