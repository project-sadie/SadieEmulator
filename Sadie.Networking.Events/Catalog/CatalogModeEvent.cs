using Sadie.Game.Catalog;
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
        var isBuildersClub = reader.ReadString() == "BUILDERS_CLUB";
        var mode = isBuildersClub ? 1 : 0;
        var pages = _catalogPageRepository.GetAll();
        
        await client.WriteToStreamAsync(new CatalogModeWriter(mode).GetAllBytes());
        await client.WriteToStreamAsync(new CatalogPagesWriter(mode, pages).GetAllBytes());
    }
}