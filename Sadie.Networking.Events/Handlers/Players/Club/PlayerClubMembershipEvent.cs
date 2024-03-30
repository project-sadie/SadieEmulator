using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Club;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players.Club;

public class PlayerClubMembershipEvent(PlayerClubMembershipParser parser) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        var subscription = client.Player?.Data.Subscriptions.FirstOrDefault(x => x.Name == parser.SubscriptionName);
        
        if (subscription == null)
        {
            return;
        }
        
        var tillExpire = subscription.Expires - subscription.Started;
        var daysLeft = (int) tillExpire.TotalDays;
        var minutesLeft = (int) tillExpire.TotalMinutes;
            
        await client.WriteToStreamAsync(new PlayerSubscriptionWriter(
            subscription.Name,
            daysLeft,
            0, 
            0, 
            0, 
            true, 
            true, 
            0, 
            0, 
            minutesLeft).GetAllBytes());
    }
}