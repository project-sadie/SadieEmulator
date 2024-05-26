using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerIds.CatalogMode)]
public class CatalogModeEventHandler(
    SadieContext dbContext) : INetworkPacketEventHandler
{
    [PacketData] public string? Mode { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var parentlessPages = await dbContext.Set<CatalogPage>()
            .Include(x => x.Pages)
            .Include(x => x.Items).ThenInclude(x => x.FurnitureItems)
            .Where(x => x.CatalogPageId == -1)
            .ToListAsync();

        await client.WriteToStreamAsync(new CatalogModeWriter
        {
            Mode = Mode == "BUILDERS_CLUB" ? 1 : 0
        });
        
        await client.WriteToStreamAsync(new CatalogTabsWriter
        {
            Mode = Mode,
            TabPages = parentlessPages
        });
    }
}