using Microsoft.EntityFrameworkCore;
using Sadie.Db;
using Sadie.Db.Models.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerId.CatalogIndex)]
public class CatalogIndexEventHandler(
    IDbContextFactory<SadieContext> dbContextFactory) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var parentlessPages = await dbContext.Set<CatalogPage>()
            .Include(x => x.Pages.OrderBy(y => y.OrderId))
            .ThenInclude(x => x.Pages.OrderBy(y => y.OrderId))
            .ThenInclude(x => x.Pages.OrderBy(y => y.OrderId))
            .Where(x => x.CatalogPageId == null)
            .ToListAsync();

        await client.WriteToStreamAsync(new CatalogTabsWriter
        {
            Mode = client.Player!.State.CatalogMode,
            TabPages = parentlessPages
        });
    }
}