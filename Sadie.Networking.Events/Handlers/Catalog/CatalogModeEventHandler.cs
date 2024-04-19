using Sadie.Game.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Catalog;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;

namespace Sadie.Networking.Events.Handlers.Catalog;

public class CatalogModeEventHandler(
    CatalogModeEventParser eventParser,
    CatalogPageRepository catalogPageRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.CatalogMode;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var mode = eventParser.Mode;
        var pages = catalogPageRepository.GetByParentId(-1);

        await client.WriteToStreamAsync(new CatalogModeWriter(mode == "BUILDERS_CLUB" ? 1 : 0));
        await client.WriteToStreamAsync(new CatalogTabsWriter(mode, pages));
    }
}