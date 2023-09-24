using Sadie.Game.Catalog;
using Sadie.Game.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Catalog;

public class CatalogPageEvent : INetworkPacketEvent
{
    private readonly CatalogPageRepository _catalogPageRepo;

    public CatalogPageEvent(CatalogPageRepository catalogPageRepo)
    {
        _catalogPageRepo = catalogPageRepo;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var pageId = reader.ReadInteger();
        var unknown1 = reader.ReadInteger();
        var catalogMode = reader.ReadString();
        
        var (exists, page) = _catalogPageRepo.TryGet(pageId);

        if (!exists || page is not { Enabled: true } || !page.Visible)
        {
            return;
        }

        await client.WriteToStreamAsync(new CatalogPageWriter(page, catalogMode).GetAllBytes());
    }
}