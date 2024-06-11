using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerIds.CatalogIndex)]
public class CatalogIndexEventHandler(SadieContext dbContext) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var parentlessPages = await dbContext.Set<CatalogPage>()
            .Include("Pages")
            .Include("Pages.Pages")
            .Include("Pages.Pages.Pages")
            .Where(x => x.CatalogPageId == -1)
            .ToListAsync();

        await client.WriteToStreamAsync(new CatalogTabsWriter
        {
            Mode = client.Player.State.CatalogMode,
            TabPages = parentlessPages
        });
    }
}