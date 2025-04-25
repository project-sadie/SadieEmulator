using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API;
using Sadie.API.Game.Players;
using Sadie.Database;
using Sadie.Database.Models.Catalog;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Database.Models.Furniture;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Catalog;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Catalog;
using Sadie.Networking.Writers.Players;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Players.Permission;
using Sadie.Networking.Writers.Players.Purse;
using Sadie.Networking.Writers.Players.Subscriptions;
using Sadie.Shared.Constants;

namespace Sadie.Networking.Events.Handlers.Catalog;

[PacketId(EventHandlerId.CatalogPurchase)]
public class CatalogPurchaseEventHandler(
    IDbContextFactory<SadieContext> dbContextFactory,
    IPlayerHelperService playerHelperService,
    IMapper mapper) : INetworkPacketEventHandler
{
    public int PageId { get; set; }
    public int ItemId { get; set; }
    public string? MetaData { get; set; }
    public int Amount { get; set; }
    
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

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
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
            await ProcessVipPurchaseAsync(client);
            return;
        }

        var catalogItem = page.Items.FirstOrDefault(x => x.Id == ItemId);

        if (catalogItem == null)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter
            {
                Error = (int) CatalogPurchaseError.Server
            });
            
            return;
        }

        if (catalogItem.RequiresClubMembership &&
            client.Player?.Subscriptions.FirstOrDefault(x => x.Subscription.Name == "HABBO_CLUB") == null)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseUnavailableWriter
            {
                Code = 1
            });
            
            return;
        }

        if (!await TryChargeForCatalogItemPurchaseAsync(client, catalogItem, Amount))
        {
            return;
        }

        if (page.Layout == CatalogPageLayout.Bots && 
            catalogItem.Name.Contains("bot_") &&
            !string.IsNullOrEmpty(catalogItem.MetaData))
        {
            await ProcessBotPurchaseAsync(client, catalogItem);
            return;
        }
        
        var created = DateTime.Now;
        var newItems = new List<PlayerFurnitureItem>();
        
        var furnitureItem = catalogItem.FurnitureItems.First();
        var mappedPlayer = mapper.Map<Player>(client.Player);

        if (catalogItem.FurnitureItems.Any(x => x.InteractionType == FurnitureItemInteractionType.Teleport))
        {
            await ProcessTeleportPurchaseAsync(client,
                mappedPlayer,
                furnitureItem,
                created,
                newItems,
                catalogItem);
            return;
        }
        
        player.State.LastCatalogPurchase = DateTime.Now;

        for (var i = 0; i < Amount; i++)
        {
            var newItem = new PlayerFurnitureItem
            {
                Player = mappedPlayer,
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
        await ConfirmPurchaseAsync(client, catalogItem);
    }

    private async Task ProcessTeleportPurchaseAsync(INetworkClient client,
        Player? mappedPlayer,
        FurnitureItem furnitureItem,
        DateTime created,
        List<PlayerFurnitureItem> newItems,
        CatalogItem catalogItem)
    {
        var parent = new PlayerFurnitureItem
        {
            Player = mappedPlayer,
            FurnitureItem = furnitureItem,
            LimitedData = "1:1",
            MetaData = MetaData,
            CreatedAt = created
        };
            
        var child = new PlayerFurnitureItem
        {
            Player = mappedPlayer,
            FurnitureItem = furnitureItem,
            LimitedData = "1:1",
            MetaData = MetaData,
            CreatedAt = created
        };
            
        client.Player.FurnitureItems.Add(parent);
        client.Player.FurnitureItems.Add(child);
            
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
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
            
        await ConfirmPurchaseAsync(client, catalogItem);
    }

    private async Task ProcessBotPurchaseAsync(INetworkClient client, CatalogItem catalogItem)
    {
        var metaData = catalogItem.MetaData;

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
            Gender = information["gender"].ToUpper() == "M" ? AvatarGender.Male : AvatarGender.Female,
            CreatedAt = DateTime.Now
        };

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        dbContext.Entry(bot).State = EntityState.Added;
        await dbContext.SaveChangesAsync();

        client.Player.Bots.Add(bot);

        await client.WriteToStreamAsync(new PlayerInventoryAddBotWriter
        {
            Id = bot.Id,
            Username = bot.Username,
            Motto = bot.Motto,
            Gender = bot.Gender == AvatarGender.Male ? "m" : "f",
            FigureCode = bot.FigureCode,
            OpenInventory = true
        });
            
        await ConfirmPurchaseAsync(client, catalogItem);
    }

    private async Task ProcessVipPurchaseAsync(INetworkClient client)
    {
        var player = client.Player;
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
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
            
            var playerData = client.Player!.Data;
            
            if (playerData!.CreditBalance < offer.CostCredits || 
                (offer.CostPointsType == 0 && playerData.PixelBalance < offer.CostPoints) ||
                (offer.CostPointsType != 0 && playerData.SeasonalBalance < offer.CostPoints))
            {
                return;
            }

            if (offer.CostCredits > 0)
            {
                playerData.CreditBalance -= offer.CostCredits;
                   
                await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter
                {
                    Credits = playerData.CreditBalance
                });
            }

            if (offer.CostPoints > 0)
            {
                if (offer.CostPointsType == 0)
                {
                    playerData.PixelBalance -= offer.CostPoints;
                }
                else
                {
                    playerData.SeasonalBalance -= offer.CostPoints;
                }
            }
                   
            await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter
            {
                Currencies = NetworkPacketEventHelpers.GetPlayerCurrencyMapFromData(playerData)
            });

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
            
            var subWriter = playerHelperService.GetSubscriptionWriterAsync(client.Player, "HABBO_CLUB");

            if (subWriter != null)
            {
                await client.WriteToStreamAsync((PlayerSubscriptionWriter) subWriter);
            }
    }

    private async Task ConfirmPurchaseAsync(INetworkObject client, CatalogItem item)
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
    
    public static async Task<bool> TryChargeForCatalogItemPurchaseAsync(INetworkClient client, CatalogItem item, int amount)
    {
        var costInCredits = item.CostCredits * amount;
        var costInPoints = item.CostPoints * amount;

        var playerData = client.Player.Data;
        
        if (playerData.CreditBalance < costInCredits || 
            (item.CostPointsType == 0 && playerData.PixelBalance < costInPoints) ||
            (item.CostPointsType != 0 && playerData.SeasonalBalance < costInPoints))
        {
            return false;
        }

        if (costInCredits > 0)
        {
            playerData.CreditBalance -= costInCredits;
        
            await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter
            {
                Credits = playerData.CreditBalance
            });
        }

        if (costInPoints > 0)
        {
            if (item.CostPointsType == 0)
            {
                playerData.PixelBalance -= costInPoints;
            }
            else
            {
                playerData.SeasonalBalance -= costInPoints;
            }
        
            await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter
            {
                Currencies = NetworkPacketEventHelpers.GetPlayerCurrencyMapFromData(playerData)
            });
        }

        return true;
    }
}