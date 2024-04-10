using Sadie.Database;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Players;
using Sadie.Game.Catalog;
using Sadie.Game.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Catalog;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Catalog;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Players.Purse;
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

        if (!await TryChargeForItemAsync(client, item))
        {
            return;
        }
        
        player.State.LastCatalogPurchase = DateTime.Now;
        
        var created = DateTime.Now;
        var newItems = new List<PlayerFurnitureItem>();
        var metaData = eventParser.ExtraData;

        switch (item.FurnitureItems.First().InteractionType)
        {
            case "roomeffect":
                if (string.IsNullOrEmpty(metaData))
                {
                    metaData = 0.ToString();
                }
                else
                {
                    metaData = double
                        .Parse(metaData)
                        .ToString()
                        .Replace(',', '.');
                }
                break;
        }

        for (var i = 0; i < eventParser.Amount; i++)
        {
            var limitedData = $"1:1"; // TODO: maxStack:maxSell

            var newItem = new PlayerFurnitureItem
            {
                PlayerId = client.Player.Id,
                FurnitureItem = item.FurnitureItems.First(),
                LimitedData = limitedData,
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

    private async Task<bool> TryChargeForItemAsync(INetworkClient client, CatalogItem item)
    {
        var costInCredits = item.CostCredits * eventParser.Amount;
        var costInPoints = item.CostPoints * eventParser.Amount;

        var playerData = client.Player.Data;
        
        if (playerData.CreditBalance < costInCredits || 
            (item.CostPointsType == 0 && playerData.PixelBalance < costInPoints) ||
            (item.CostPointsType != 0 && playerData.SeasonalBalance < costInPoints))
        {
            return false;
        }

        playerData.CreditBalance -= costInCredits;

        if (item.CostPointsType == 0)
        {
            playerData.PixelBalance -= costInPoints;
        }
        else
        {
            playerData.SeasonalBalance -= costInPoints;
        }

        var currencies = new Dictionary<int, long>
        {
            {0, playerData.PixelBalance},
            {1, 0}, // snowflakes
            {2, 0}, // hearts
            {3, 0}, // gift points
            {4, 0}, // shells
            {5, playerData.SeasonalBalance},
            {101, 0}, // snowflakes
            {102, 0}, // unknown
            {103, playerData.GotwPoints},
            {104, 0}, // unknown
            {105, 0} // unknown
        };
        
        await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter(playerData.CreditBalance).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter(currencies).GetAllBytes());

        return true;
    }
}