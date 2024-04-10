using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Catalog;
using Sadie.Game.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Catalog;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Shared;

namespace Sadie.Networking.Events.Handlers.Catalog;

public class CatalogPurchaseEventHandler(
    CatalogPurchaseEventParser eventParser,
    SadieContext dbContext,
    CatalogPageRepository pageRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.CatalogPurchase;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var player = client.Player;

        if (client.Player == null)
        {
            return;
        }
        
        if ((DateTime.Now - player!.State.LastPlayerSearch).TotalMilliseconds < CooldownIntervals.CatalogPurchase)
        {
            var bytes = new CatalogPurchaseFailedWriter((int)CatalogPurchaseError.Server).GetAllBytes();
            
            await client.WriteToStreamAsync(bytes);
            return;
        }

        var page = pageRepository.TryGet(eventParser.PageId);

        if (page == null)
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

        if (!await NetworkPacketEventHelpers.TryChargeForCatalogItemPurchaseAsync(client, item, eventParser.Amount))
        {
            return;
        }
        
        player.State.LastCatalogPurchase = DateTime.Now;
        
        var created = DateTime.Now;
        var newItems = new List<PlayerFurnitureItem>();
        var metaData = NetworkPacketEventHelpers.CalculateMetaDataForCatalogItem(eventParser.ExtraData, item);

        for (var i = 0; i < eventParser.Amount; i++)
        {
            var newItem = new PlayerFurnitureItem
            {
                PlayerId = client.Player.Id,
                FurnitureItem = item.FurnitureItems.First(),
                LimitedData = $"1:1", // TODO: maxStack:maxSell
                MetaData = metaData,
                CreatedAt = created
            };
            
            newItems.Add(newItem);
            client.Player.FurnitureItems.Add(newItem);
        }

        dbContext.PlayerFurnitureItems.AddRange(newItems);
        await dbContext.SaveChangesAsync();
        
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
            item.MetaData,
            false,
            0,
            0,
            item.FurnitureItems
            ).GetAllBytes());
        
        await client.WriteToStreamAsync(new PlayerInventoryRefreshWriter().GetAllBytes());
    }
}