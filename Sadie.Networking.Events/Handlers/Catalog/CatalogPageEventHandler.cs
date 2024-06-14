using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerIds.CatalogPage)]
public class CatalogPageEventHandler(SadieContext dbContext) : INetworkPacketEventHandler
{
    [PacketData] public int PageId { get; set; }
    [PacketData] public int OfferId { get; set; }
    [PacketData] public string? CatalogMode { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var page = await dbContext
            .Set<CatalogPage>()
            .Include("Pages")
            .Include("Pages.Pages")
            .Include("Pages.Pages.Pages")
            .Include(c => c.Items).ThenInclude(x => x.FurnitureItems)
            .Where(x => x.Id == PageId)
            .FirstOrDefaultAsync();

        if (page is not { Enabled: true } || !page.Visible)
        {
            return;
        }

        var frontPageItems = await dbContext
            .Set<CatalogFrontPageItem>()
            .Include(x => x.CatalogPage)
            .ToListAsync();

        await client.WriteToStreamAsync(new CatalogPageWriter
        {
            PageId = page.Id,
            PageLayout = page.Layout,
            Images = page.ImagesJson.Where(x => !string.IsNullOrEmpty(x)).ToList(),
            Texts = page.TextsJson.Where(x => !string.IsNullOrEmpty(x)).ToList(),
            Items = page.Items.ToList(),
            CatalogMode = CatalogMode,
            AcceptSeasonCurrencyAsCredits = false,
            FrontPageItems = frontPageItems
        });
    }
}