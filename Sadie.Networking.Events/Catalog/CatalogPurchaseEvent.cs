using Sadie.Game.Catalog;
using Sadie.Game.Navigator;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;
using Sadie.Networking.Writers.Players;
using Sadie.Shared;

namespace Sadie.Networking.Events.Catalog;

public class CatalogPurchaseEvent : INetworkPacketEvent
{
    private readonly CatalogPageRepository _pageRepository;

    public CatalogPurchaseEvent(CatalogPageRepository pageRepository)
    {
        _pageRepository = pageRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if ((DateTime.Now - client.Player.State.LastPlayerSearch).TotalSeconds < CooldownSeconds.CatalogPurchase)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter(CatalogPurchaseError.Server).GetAllBytes());
            return;
        }
        
        client.Player.State.LastPlayerSearch = DateTime.Now;

        var pageId = reader.ReadInteger();
        var itemId = reader.ReadInteger();
        var extraData = reader.ReadString();
        var amount = reader.ReadInteger();

        var (found, page) = _pageRepository.TryGet(pageId);

        if (!found)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter(CatalogPurchaseError.Server).GetAllBytes());
            return;
        }

        var item = page.Items.FirstOrDefault(x => x.Id == itemId);

        if (item == null)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter(CatalogPurchaseError.Server).GetAllBytes());
            return;
        }

        var furnitureItem = item.FurnitureItem;
        
        if (furnitureItem.Type is 'p' or 'e')
        {
            await client.WriteToStreamAsync(new PlayerMotdMessageWriter(new List<string> { "Purchasing pets and/or effects is coming soon" }).GetAllBytes());
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter(CatalogPurchaseError.Server).GetAllBytes());
            return;
        }

        // TODO: validate amount
        // TODO: validate can afford
    }
}