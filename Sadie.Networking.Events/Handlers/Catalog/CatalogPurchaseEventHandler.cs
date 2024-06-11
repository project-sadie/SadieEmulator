using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Catalog;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Database.Models.Players;
using Sadie.Game.Catalog;
using Sadie.Game.Catalog.Pages;
using Sadie.Game.Players.Packets;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Catalog;
using Sadie.Shared;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerIds.CatalogPurchase)]
public class CatalogPurchaseEventHandler(
    SadieContext dbContext) : INetworkPacketEventHandler
{
    [PacketData] public int PageId { get; set; }
    [PacketData] public int ItemId { get; set; }
    [PacketData] public string? ExtraData { get; set; }
    [PacketData] public int Amount { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
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
            .FirstOrDefaultAsync(x => x.Id == PageId);

        if (page == null)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter
            {
                Error = (int) CatalogPurchaseError.Server
            });
            
            return;
        }

        if (page.Layout == CatalogPageLayout.VipBuy)
        {
            var offer = await dbContext
                .Set<CatalogClubOffer>()
                .FirstOrDefaultAsync(x => x.Id == ItemId);

            if (offer == null)
            {
                await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter
                {
                    Error = (int) CatalogPurchaseError.Server
                });
                
                return;
            }

            var clubSubscription = await dbContext
                .Subscriptions
                .Where(x => x.Name == "HABBO_CLUB")
                .FirstOrDefaultAsync();

            if (clubSubscription == null)
            {
                return;
            }

            if (!await RoomHelpers.TryChargeForClubOfferPurchaseAsync(client, offer))
            {
                return;
            }

            var subscription = new PlayerSubscription
            {
                PlayerId = player.Id,
                SubscriptionId = clubSubscription.Id,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(offer.DurationDays)
            };

            player.Subscriptions.Add(subscription);
            
            dbContext.PlayerSubscriptions.Add(subscription);
            await dbContext.SaveChangesAsync();
            
            await client.WriteToStreamAsync(new CatalogPurchaseOkWriter
            {
                Id = offer.Id,
                Name = offer.Name,
                Rented = false,
                CostCredits = offer.CostCredits,
                CostPoints = offer.CostPoints,
                CostPointsType = offer.CostPointsType,
                CanGift = true,
                FurnitureItems = [],
                Amount = Amount,
                ClubLevel = 0,
                CanPurchaseBundles = false,
                Metadata = "",
                IsLimited = false,
                LimitedItemSeriesSize = 0,
                AmountLeft = 0
            });
        
            await client.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
            return;
        }

        var item = page.Items.FirstOrDefault(x => x.Id == ItemId);

        if (item == null)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter
            {
                Error = (int) CatalogPurchaseError.Server
            });
            
            return;
        }

        if (item.RequiresClubMembership &&
            client.Player?.Subscriptions.FirstOrDefault(x => x.Subscription.Name == "HABBO_CLUB") == null)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseUnavailableWriter
            {
                Code = 1
            });
            
            return;
        }

        if (!await RoomHelpers.TryChargeForCatalogItemPurchaseAsync(client, item, Amount))
        {
            return;
        }

        if (page.Layout == CatalogPageLayout.Bots)
        {
            var bot = new PlayerBot
            {
                PlayerId = client.Player.Id,
                RoomId = null,
                Username = "",
                FigureCode = "",
                Motto = "",
                Gender = AvatarGender.Male
            };

            dbContext.PlayerBots.Add(bot);
            await dbContext.SaveChangesAsync();

            player.Bots.Add(bot);
            
            await ConfirmPurchaseAsync(client, item);
            return;
        }
        
        player.State.LastCatalogPurchase = DateTime.Now;
        
        var created = DateTime.Now;
        var newItems = new List<PlayerFurnitureItem>();
        var furnitureItem = item.FurnitureItems.First();

        for (var i = 0; i < Amount; i++)
        {
            var newItem = new PlayerFurnitureItem
            {
                PlayerId = client.Player.Id,
                FurnitureItem = furnitureItem,
                LimitedData = "1:1",
                MetaData = ExtraData,
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
        await ConfirmPurchaseAsync(client, item);
    }

    private async Task ConfirmPurchaseAsync(INetworkClient client, CatalogItem item)
    {
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
            Amount = Amount,
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