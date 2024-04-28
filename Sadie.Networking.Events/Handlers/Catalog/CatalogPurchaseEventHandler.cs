using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Database.Models.Players;
using Sadie.Game.Catalog;
using Sadie.Game.Players.Packets;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Catalog;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;
using Sadie.Shared;

namespace Sadie.Networking.Events.Handlers.Catalog;

public class CatalogPurchaseEventHandler(
    CatalogPurchaseEventParser eventParser,
    SadieContext dbContext) : INetworkPacketEventHandler
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
            var bytes = new CatalogPurchaseFailedWriter
            {
                Error = (int) CatalogPurchaseError.Server
            };
            
            await client.WriteToStreamAsync(bytes);
            return;
        }

        var page = await dbContext
            .Set<CatalogPage>()
            .Include(catalogPage => catalogPage.Items)
            .ThenInclude(catalogItem => catalogItem.FurnitureItems)
            .FirstOrDefaultAsync(x => x.Id == eventParser.PageId);

        if (page == null)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter
            {
                Error = (int) CatalogPurchaseError.Server
            });
            
            return;
        }

        var item = page.Items.FirstOrDefault(x => x.Id == eventParser.ItemId);

        if (item == null)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter
            {
                Error = (int) CatalogPurchaseError.Server
            });
            
            return;
        }

        if (!await NetworkPacketEventHelpers.TryChargeForCatalogItemPurchaseAsync(client, item, eventParser.Amount))
        {
            return;
        }
        
        player.State.LastCatalogPurchase = DateTime.Now;
        
        var created = DateTime.Now;
        var newItems = new List<PlayerFurnitureItem>();
        var furnitureItem = item.FurnitureItems.First();

        for (var i = 0; i < eventParser.Amount; i++)
        {
            var newItem = new PlayerFurnitureItem
            {
                PlayerId = client.Player.Id,
                FurnitureItem = furnitureItem,
                LimitedData = $"1:1",
                MetaData = eventParser.ExtraData,
                CreatedAt = created
            };
            
            client.Player.FurnitureItems.Add(newItem);
            dbContext.Entry(newItem).State = EntityState.Added;
            newItems.Add(newItem);
        }

        await dbContext.SaveChangesAsync();

        var writer = new PlayerInventoryAddItemsWriter
        {
            Items = newItems
        };
        
        await client.WriteToStreamAsync(writer);
        
        await client.WriteToStreamAsync(new CatalogPurchaseOkWriter
        {
            Id = item.Id,
            Name = item.Name,
            Rented = false,
            CostCredits = item.CostCredits,
            CostPoints = item.CostPoints,
            CostPointsType = item.CostPointsType,
            CanGift = item.FurnitureItems.First().CanGift,
            FurnitureItems = item.FurnitureItems,
            Amount = eventParser.Amount,
            ClubLevel = item.RequiresClubMembership ? 1 : 0,
            CanPurchaseBundles = item.Amount != 1,
            Metadata = item.MetaData,
            IsLimited = false,
            LimitedItemSeriesSize = 0,
            AmountLeft = 0
        });
        
        await client.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
    }
}