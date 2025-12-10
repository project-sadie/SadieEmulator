using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Players;
using Sadie.API.Interfaces.Game.Catalog;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.Core.Enums.Game.Catalog;
using Sadie.Db;
using Sadie.Db.Models.Catalog;
using Sadie.Db.Models.Players;
using Sadie.Networking.Events;
using Sadie.Networking.Writers.Catalog;
using Sadie.Networking.Writers.Players.Permission;
using Sadie.Networking.Writers.Players.Purse;
using Sadie.Networking.Writers.Players.Subscriptions;

namespace Sadie.Game.Catalog;

public class VipPurchaseProcessor(IDbContextFactory<SadieDbContext> dbContextFactory,
    IPlayerHelperService playerHelperService,
    IMapper mapper) : IVipPurchaseProcessor
{
    public async Task ProcessVipPurchaseAsync(INetworkClient client, int itemId)
    {
        var player = client.Player;
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var offer = await dbContext
                .Set<CatalogClubOffer>()
                .FirstOrDefaultAsync(x => x.Id == itemId);

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
            
            var playerData = client.Player!.Player.Data;
            
            if (playerData.CreditBalance < offer.CostCredits || 
                (offer.CostPointsType == 0 && playerData.PixelBalance < offer.CostPoints) ||
                (offer.CostPointsType != 0 && playerData.SeasonalBalance < offer.CostPoints))
            {
                return;
            }

            if (offer.CostCredits > 0)
            {
                client.Player!.Player.Data.CreditBalance -= offer.CostCredits;
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

            var subscription = new PlayerSubscriptionDto
            {
                PlayerId = player.Player.Id,
                SubscriptionId = clubSubscription.Id,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(offer.DurationDays)
            };

            player.Player.Subscriptions.Add(subscription);

            var subscriptionEntity = mapper.Map<PlayerSubscription>(subscription);
            dbContext.PlayerSubscriptions.Add(subscriptionEntity);
            
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
                Rank = player.Player.Roles.Count != 0 ? player.Player.Roles.Max(x => x.Id) : 1,
                Ambassador = true
            });
            
            var subWriter = playerHelperService.GetSubscriptionWriterAsync(client.Player, "HABBO_CLUB");

            if (subWriter != null)
            {
                await client.WriteToStreamAsync((PlayerSubscriptionWriter) subWriter);
            }
    }
}