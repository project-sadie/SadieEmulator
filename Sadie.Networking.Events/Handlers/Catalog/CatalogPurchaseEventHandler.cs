using Sadie.Game.Catalog;
using Sadie.Game.Catalog.Items;
using Sadie.Game.Catalog.Pages;
using Sadie.Game.Furniture;
using Sadie.Game.Players.Inventory;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Catalog;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;
using Sadie.Networking.Writers.Players;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Players.Purse;
using Sadie.Shared;

namespace Sadie.Networking.Events.Handlers.Catalog;

public class CatalogPurchaseEventHandler(
    CatalogPurchaseEventParser eventParser,
    CatalogPageRepository pageRepository, 
    IPlayerInventoryDao inventoryDao) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.CatalogPurchase;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var player = client.Player;

        if ((DateTime.Now - player!.State.LastPlayerSearch).TotalMilliseconds < CooldownIntervals.CatalogPurchase)
        {
            var bytes = new CatalogPurchaseFailedWriter((int)CatalogPurchaseError.Server).GetAllBytes();
            
            await client.WriteToStreamAsync(bytes);
            return;
        }

        var (found, page) = pageRepository.TryGet(eventParser.PageId);

        if (!found || page == null)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter((int) CatalogPurchaseError.Server).GetAllBytes());
            return;
        }

        var item = page.Items.FirstOrDefault(x => x.Id == eventParser.ItemId);

        if (item == null)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter((int) CatalogPurchaseError.Server).GetAllBytes());
            return;
        }

        if (!await TryChargeForItemAsync(client, item))
        {
            return;
        }
        
        player.State.LastCatalogPurchase = DateTime.Now;
        
        var created = DateTime.Now;
        var newItems = new List<PlayerInventoryFurnitureItem>();

        for (var i = 0; i < eventParser.Amount; i++)
        {
            var limitedData = $"1:1"; // TODO: maxStack:maxSell
            newItems.Add(new PlayerInventoryFurnitureItem(0, item.FurnitureItems.First(), limitedData, item.Metadata, created));
        }

        foreach (var newItem in newItems)
        {
            newItem.Id = await inventoryDao.CreateItemAsync(client.Player.Data.Id, newItem);
        }

        client.Player.Data.Inventory.AddItems(newItems);
        
        await client.WriteToStreamAsync(new PlayerInventoryAddItemsWriter(newItems).GetAllBytes());
        
        await client.WriteToStreamAsync(new CatalogPurchaseOkWriter(
            item.Id,
            item.Name,
            false,
            item.CostCredits,
            item.CostPoints,
            item.CostPointsType,
            item.FurnitureItems.First().CanGift,
            eventParser.Amount,
            item.RequiresClubMembership ? 1 : 0,
            item.Amount != 1,
            item.Metadata,
            false,
            0,
            0,
            item.FurnitureItems
            ).GetAllBytes());
        
        await client.WriteToStreamAsync(new PlayerInventoryRefreshWriter().GetAllBytes());
    }

    private async Task<bool> TryChargeForItemAsync(INetworkClient client, CatalogItem item)
    {
        var costInCredits = item.CostCredits * eventParser.Amount;
        var costInPoints = item.CostPoints * eventParser.Amount;
        var balance = client.Player.Data.Balance;
        
        if (balance.Credits < costInCredits || 
            (item.CostPointsType == 0 && balance.Pixels < costInPoints) ||
            (item.CostPointsType != 0 && balance.Seasonal < costInPoints))
        {
            return false;
        }

        balance.Credits -= costInCredits;

        if (item.CostPointsType == 0)
        {
            balance.Pixels -= costInPoints;
        }
        else
        {
            balance.Seasonal -= costInPoints;
        }

        var currencies = new Dictionary<int, long>
        {
            {0, balance.Pixels},
            {1, 0}, // snowflakes
            {2, 0}, // hearts
            {3, 0}, // gift points
            {4, 0}, // shells
            {5, balance.Seasonal},
            {101, 0}, // snowflakes
            {102, 0}, // unknown
            {103, balance.Gotw},
            {104, 0}, // unknown
            {105, 0} // unknown
        };
        
        await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter(balance.Credits).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter(currencies).GetAllBytes());

        return true;
    }
}