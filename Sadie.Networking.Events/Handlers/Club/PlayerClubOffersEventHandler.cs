using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Catalog;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Db.Models.Catalog;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Club;

[PacketId(EventHandlerId.HabboClubData)]
public class PlayerClubOffersEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper) : INetworkPacketEventHandler
{
    public int WindowId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null)
        {
            return;
        }

        var daysRemaining = 0;
        
        var clubSubscription = client
            .Player
            .Subscriptions
            .FirstOrDefault(x => x.Subscription.Name == "HABBO_CLUB");

        if (clubSubscription != null)
        {
            var daysTotal = (clubSubscription.ExpiresAt - clubSubscription.CreatedAt).TotalDays;
            var daysSinceStarted = (DateTime.Now - clubSubscription.CreatedAt).TotalDays;

            daysRemaining = (int)(daysTotal - daysSinceStarted);
        }
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var catalogClubOffers = await dbContext
            .Set<CatalogClubOffer>()
            .ToListAsync();
        
        await client.WriteToStreamAsync(new PlayerClubOffersWriter
        {
            Offers = mapper.Map<IReadOnlyCollection<CatalogClubOfferDto>>(catalogClubOffers),
            WindowId = WindowId,
            Unused = false,
            CanGift = false,
            RemainingDays = daysRemaining
        });
    }
}