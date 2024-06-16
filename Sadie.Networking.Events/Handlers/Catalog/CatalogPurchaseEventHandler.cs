using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Catalog;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Game.Catalog;
using Sadie.Game.Catalog.Pages;
using Sadie.Game.Players;
using Sadie.Game.Players.Packets;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Catalog;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Players.Permission;
using Sadie.Shared;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerIds.CatalogPurchase)]
public class CatalogPurchaseEventHandler(
    SadieContext dbContext) : INetworkPacketEventHandler
{
    [PacketData] public int PageId { get; set; }
    [PacketData] public int ItemId { get; set; }
    [PacketData] public string? MetaData { get; set; }
    [PacketData] public int Amount { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
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

            if (!await RoomHelpersDirty.TryChargeForClubOfferPurchaseAsync(client, offer))
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
                Id = 0,
                Name = "",
                Rented = false,
                CostCredits = 0,
                CostPoints = 0,
                CostPointsType = 0,
                CanGift = false,
                FurnitureItems = [],
                Amount = 0,
                ClubLevel = 0,
                CanPurchaseBundles = false,
                Metadata = null,
                IsLimited = false,
                LimitedItemSeriesSize = 0,
                AmountLeft = 0
            });

            await client.WriteToStreamAsync(new PlayerPermissionsWriter
            {
                Club = 2,
                Rank = player.Roles.Count != 0 ? player.Roles.Max(x => x.Id) : 1,
                Ambassador = true
            });
            
            var subWriter = PlayerHelpersDirty.GetSubscriptionWriterAsync(client.Player, "HABBO_CLUB");

            if (subWriter != null)
            {
                await client.WriteToStreamAsync(subWriter);
            }

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

        if (!await RoomHelpersDirty.TryChargeForCatalogItemPurchaseAsync(client, item, Amount))
        {
            return;
        }

        if (page.Layout == CatalogPageLayout.Bots && 
            item.Name.Contains("bot_") &&
            !string.IsNullOrEmpty(item.MetaData))
        {
            var metaData = item.MetaData;

            if (string.IsNullOrEmpty(metaData))
            {
                return;
            }
            
            var information = metaData
                .Split(";")
                .ToDictionary(k => k.Split(":")[0], v => v.Split(":")[1]);

            var bot = new PlayerBot
            {
                PlayerId = client.Player.Id,
                RoomId = null,
                Username = information["name"],
                FigureCode = information["figure"],
                Motto = information["motto"],
                Gender = information["gender"].ToUpper() == "M" ? AvatarGender.Male : AvatarGender.Female
            };

            dbContext.Entry(bot).State = EntityState.Added;
            await dbContext.SaveChangesAsync();

            client.Player.Bots.Add(bot);

            await client.WriteToStreamAsync(new PlayerInventoryAddBotWriter
            {
                Id = bot.Id,
                Username = bot.Username,
                Motto = bot.Motto,
                Gender = bot.Gender == AvatarGender.Male ? "m" : "f",
                FigureCode = bot.FigureCode
            });
            
            await ConfirmPurchaseAsync(client, item);
            return;
        }
        
        var created = DateTime.Now;
        var newItems = new List<PlayerFurnitureItem>();
        
        var furnitureItem = item.FurnitureItems.First();

        if (item.FurnitureItems.Any(x => x.InteractionType == "teleport"))
        {
            var parent = new PlayerFurnitureItem
            {
                Player = client.Player,
                FurnitureItem = furnitureItem,
                LimitedData = "1:1",
                MetaData = MetaData,
                CreatedAt = created
            };
            
            var child = new PlayerFurnitureItem
            {
                Player = client.Player,
                FurnitureItem = furnitureItem,
                LimitedData = "1:1",
                MetaData = MetaData,
                CreatedAt = created
            };
            
            client.Player.FurnitureItems.Add(parent);
            client.Player.FurnitureItems.Add(child);
            
            dbContext.Entry(parent).State = EntityState.Added;
            dbContext.Entry(child).State = EntityState.Added;
            
            newItems.AddRange([parent, child]);

            await dbContext.SaveChangesAsync();

            dbContext.PlayerFurnitureItemLinks.Add(new PlayerFurnitureItemLink
            {
                ParentId = parent.Id,
                ChildId = child.Id
            });

            await dbContext.SaveChangesAsync();

            await client.WriteToStreamAsync(new PlayerInventoryUnseenItemsWriter
            {
                Count = newItems.Count,
                Category = 1,
                FurnitureItems = newItems
            });
            
            await ConfirmPurchaseAsync(client, item);
            
            return;
        }
        
        player.State.LastCatalogPurchase = DateTime.Now;

        for (var i = 0; i < Amount; i++)
        {
            var newItem = new PlayerFurnitureItem
            {
                Player = client.Player,
                FurnitureItem = furnitureItem,
                LimitedData = "1:1",
                MetaData = MetaData,
                CreatedAt = created
            };
            
            client.Player.FurnitureItems.Add(newItem);
            dbContext.Entry(newItem).State = EntityState.Added;
            newItems.Add(newItem);
        }

        await dbContext.SaveChangesAsync();

        var writer = new PlayerInventoryUnseenItemsWriter
        {
            Count = newItems.Count,
            Category = 1,
            FurnitureItems = newItems
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