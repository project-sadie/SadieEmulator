using Microsoft.EntityFrameworkCore;
using Sadie.Db;
using Sadie.Db.Models.Catalog.Pages;
using Sadie.Db.Models.Players;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Club;

[PacketId(EventHandlerId.HabboClubGifts)]
public class HabboClubGiftsEventHandler(
    IDbContextFactory<SadieContext> dbContextFactory) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null)
        {
            return;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var clubGiftPage = await dbContext
            .Set<CatalogPage>()
            .IgnoreAutoIncludes()
            .FirstOrDefaultAsync(x => x.Layout == "club_gift");

        var daysAsClub = CalculateDaysAsClub(client.Player.Subscriptions);
        var daysTillNextClubGift = daysAsClub * 86400 / 2678400 * 2678400 - daysAsClub * 86400;
        var unclaimedGifts = daysAsClub * 86400 / 2678400 * 2678400 - daysAsClub * 86400; 
        
        await client.WriteToStreamAsync(new HabboClubGiftsWriter
        {
            DaysTillNext = daysTillNextClubGift,
            UnclaimedGifts = unclaimedGifts,
            DaysAsClub = daysAsClub,
            ClubGiftPage = clubGiftPage
        });
    }

    private int CalculateDaysAsClub(ICollection<PlayerSubscription> subscriptions)
    {
        var days = 0;

        foreach (var subscription in subscriptions)
        {
            if (subscription.ExpiresAt >= DateTime.Now)
            {
                days += (int) (subscription.ExpiresAt - subscription.CreatedAt).TotalDays;
            }
            else
            {
                days += (int) (DateTime.Now - subscription.CreatedAt).TotalDays;
            }
        }

        return days;
    }
}