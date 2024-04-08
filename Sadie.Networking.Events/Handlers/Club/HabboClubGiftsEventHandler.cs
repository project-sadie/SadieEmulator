using Sadie.Database.Models.Players;
using Sadie.Game.Catalog.Pages;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Club;

public class HabboClubGiftsEventHandler(CatalogPageRepository catalogPageRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.HabboClubGifts;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null)
        {
            return;
        }
        
        var (_, clubGiftPage) = catalogPageRepository.TryGetByLayout("club_gift");

        var daysAsClub = CalculateDaysAsClub(client.Player.Subscriptions);
        var daysTillNextClubGift = daysAsClub * 86400 / 2678400 * 2678400 - daysAsClub * 86400;
        var claimedGifts = 0;
        var unclaimedGifts = daysAsClub * 86400 / 2678400 * 2678400 - daysAsClub * 86400; 
        
        await client.WriteToStreamAsync(new HabboClubGiftsWriter(
            daysTillNextClubGift, 
            unclaimedGifts, 
            daysAsClub, 
            clubGiftPage).GetAllBytes());
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