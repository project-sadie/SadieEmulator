using Sadie.Game.Catalog.Club;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Club;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Club;

public class PlayerClubOffersEventHandler(
    PlayerClubOffersEventParser eventParser, 
    CatalogClubOfferRepository clubOfferRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.HabboClubData;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

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

        await client.WriteToStreamAsync(new PlayerClubOffersWriter(
            clubOfferRepository.Offers,
            eventParser.WindowId,
            false,
            false,
            daysRemaining).GetAllBytes());
    }
}