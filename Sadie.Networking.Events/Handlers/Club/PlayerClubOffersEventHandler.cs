using Sadie.Game.Players.Club;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Club;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Club;

public class PlayerClubOffersEventHandler(PlayerClubOffersEventParser eventParser, PlayerClubOfferRepository clubOfferRepository) : INetworkPacketEventHandler
{
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
            .Data
            .Subscriptions
            .FirstOrDefault(x => x.Name == "HABBO_CLUB");

        if (clubSubscription != null)
        {
            var daysTotal = (clubSubscription.Expires - clubSubscription.Started).TotalDays;
            var daysSinceStarted = (DateTime.Now - clubSubscription.Started).TotalDays;

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