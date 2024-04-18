using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Club;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players.Club;

public class PlayerSubscriptionEventHandler(PlayerSubscriptionEventParser eventParser) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerSubscription;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var playerSub = client.Player?.Subscriptions.FirstOrDefault(x => x.Subscription.Name == eventParser.Name);
        
        if (playerSub == null)
        {
            return;
        }
        
        var tillExpire = playerSub.ExpiresAt - playerSub.CreatedAt;
        var daysLeft = (int) tillExpire.TotalDays;
        var minutesLeft = (int) tillExpire.TotalMinutes;
        var lastMod = client.Player.State.LastSubscriptionModification;
            
        await client.WriteToStreamAsync(new PlayerSubscriptionWriter(
            playerSub.Subscription.Name,
            daysLeft,
            1, 
            2, 
            0, 
            true, 
            true, 
            0, 
            0, 
            minutesLeft,
            (int)(DateTime.Now - lastMod).TotalMinutes));

        client.Player.State.LastSubscriptionModification = DateTime.Now;
    }
}