using Sadie.Game.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Catalog;

public class CatalogModeEvent(CatalogPageRepository catalogPageRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var mode = reader.ReadString();
        var pages = catalogPageRepository.GetByParentId(-1);
        
        await client.WriteToStreamAsync(new CatalogModeWriter(mode == "BUILDERS_CLUB" ? 1 : 0).GetAllBytes());
        await client.WriteToStreamAsync(new CatalogTabsWriter(mode, pages).GetAllBytes());
    }
}