using Microsoft.EntityFrameworkCore;
using Sadie.Db;
using Sadie.Db.Models.Catalog;
using Sadie.Networking.Client;
using Sadie.Networking.Writers.Players.Other;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Club;

[PacketId(EventHandlerId.HabboClubData)]
public class PlayerClubOffersEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory) : INetworkPacketEventHandler
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
            Offers = catalogClubOffers,
            WindowId = WindowId,
            Unused = false,
            CanGift = false,
            RemainingDays = daysRemaining
        });
    }
}