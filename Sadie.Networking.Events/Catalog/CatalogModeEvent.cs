using Sadie.Game.Catalog;
using Sadie.Game.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Catalog;

public class CatalogModeEvent : INetworkPacketEvent
{
    private readonly CatalogPageRepository _catalogPageRepository;

    public CatalogModeEvent(CatalogPageRepository catalogPageRepository)
    {
        _catalogPageRepository = catalogPageRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var mode = reader.ReadString();
        
        var pages = _catalogPageRepository
            .GetAll()
            .Where(x => x.ParentId == -1)
            .ToList();
        
        await client.WriteToStreamAsync(new CatalogModeWriter(mode == "BUILDERS_CLUB" ? 1 : 0).GetAllBytes());
        await client.WriteToStreamAsync(new CatalogTabsWriter(mode, pages).GetAllBytes());
    }
}