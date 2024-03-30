using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Club;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players.Club;

public class PlayerSubscriptionEvent(PlayerSubscriptionParser parser) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        var subscription = client.Player?.Data.Subscriptions.FirstOrDefault(x => x.Name == parser.Name);
        
        if (subscription == null)
        {
            return;
        }
        
        var tillExpire = subscription.Expires - subscription.Started;
        var daysLeft = (int) tillExpire.TotalDays;
        var minutesLeft = (int) tillExpire.TotalMinutes;
        var lastMod = client.Player.State.LastSubscriptionModification;
            
        await client.WriteToStreamAsync(new PlayerSubscriptionWriter(
            subscription.Name,
            daysLeft,
            1, 
            2, 
            0, 
            true, 
            true, 
            0, 
            0, 
            minutesLeft,
            (int)(DateTime.Now - lastMod).TotalMinutes).GetAllBytes());

        client.Player.State.LastSubscriptionModification = DateTime.Now;
    }
}