using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerId.CatalogIndex)]
public class CatalogIndexEventHandler(SadieContext dbContext) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var maxRole = client.Player?.Roles.MaxBy(x => x.Id)?.Id ?? 0;
        
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