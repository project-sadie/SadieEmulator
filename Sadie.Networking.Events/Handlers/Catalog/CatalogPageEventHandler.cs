using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Networking.Serialization.Attributes;
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