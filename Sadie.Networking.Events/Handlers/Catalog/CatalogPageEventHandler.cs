using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Db.Models.Catalog.FrontPage;
using Sadie.Db.Models.Catalog.Pages;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerId.CatalogPage)]
public class CatalogPageEventHandler(
    List<CatalogFrontPageItem> catalogFrontPageItems,
    List<CatalogPage> catalogPages) : INetworkPacketEventHandler
{
    public int PageId { get; set; }
    public int OfferId { get; set; }
    public string? CatalogMode { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var page = catalogPages
            .FirstOrDefault(x => x.Id == PageId);

        if (page is not { Enabled: true } || !page.Visible)
        {
            return;
        }

        await client.WriteToStreamAsync(new CatalogPageWriter
        {
            PageId = page.Id,
            PageLayout = page.Layout,
            Images = page.ImagesJson,
            Texts = page.TextsJson,
            Items = page.Items.ToList(),
            CatalogMode = CatalogMode,
            AcceptSeasonCurrencyAsCredits = false,
            FrontPageItems = catalogFrontPageItems,
            Unknown = -1
        });
    }
}