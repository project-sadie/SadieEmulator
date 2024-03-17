using Sadie.Game.Catalog;
using Sadie.Game.Catalog.Pages;
using Sadie.Game.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;
using Sadie.Networking.Writers.Players;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Shared;

namespace Sadie.Networking.Events.Catalog;

public class CatalogPurchaseEvent(CatalogPageRepository pageRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;

        if ((DateTime.Now - player.State.LastPlayerSearch).TotalSeconds < CooldownSeconds.CatalogPurchase)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter((int) CatalogPurchaseError.Server).GetAllBytes());
            return;
        }
        
        client.Player.State.LastPlayerSearch = DateTime.Now;

        var pageId = reader.ReadInteger();
        var itemId = reader.ReadInteger();
        var extraData = reader.ReadString();
        var amount = reader.ReadInteger();

        var (found, page) = pageRepository.TryGet(pageId);

        if (!found || page == null)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter((int) CatalogPurchaseError.Server).GetAllBytes());
            return;
        }

        var item = page.Items.FirstOrDefault(x => x.Id == itemId);

        if (item == null)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter((int) CatalogPurchaseError.Server).GetAllBytes());
            return;
        }

        var furnitureItem = item.FurnitureItems.First();
        
        if (furnitureItem.Type is FurnitureItemType.Pet or FurnitureItemType.Effect)
        {
            await client.WriteToStreamAsync(new PlayerMotdMessageWriter(new List<string> { "Purchasing pets and/or effects is coming soon" }).GetAllBytes());
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter((int) CatalogPurchaseError.Server).GetAllBytes());
        }

        var costInCredits = item.CostCredits * amount;
        var costInPoints = item.CostPoints * amount;

        var balance = client.Player.Data.Balance;
        
        if (balance.Credits < costInCredits || 
            (item.CostPointsType == 0 && balance.Pixels < costInPoints) ||
            (item.CostPointsType != 0 && balance.Seasonal < costInPoints))
        {
            return;
        }

        await client.WriteToStreamAsync(new CatalogPurchaseOkWriter(item).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerInventoryRefreshWriter().GetAllBytes());
    }
}