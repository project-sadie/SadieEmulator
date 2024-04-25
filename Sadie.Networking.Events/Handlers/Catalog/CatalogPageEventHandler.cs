using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Catalog;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

public class CatalogPageEventHandler(CatalogPageEventParser eventParser, 
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.CatalogPage;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var page = await dbContext
            .Set<CatalogPage>()
            .Where(x => x.Id == eventParser.PageId)
            .Include(catalogPage => catalogPage.Items)
            .FirstOrDefaultAsync();

        if (page is not { Enabled: true } || !page.Visible)
        {
            return;
        }

        var frontPageItems = await dbContext
            .Set<CatalogFrontPageItem>()
            .ToListAsync();

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
            frontPageItems));
    }
}