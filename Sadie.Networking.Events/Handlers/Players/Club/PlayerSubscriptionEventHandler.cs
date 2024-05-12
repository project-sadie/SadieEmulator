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
            
        await client.WriteToStreamAsync(new PlayerSubscriptionWriter
        {
            Name = playerSub.Subscription.Name,
            DaysLeft = daysLeft,
            MemberPeriods = 1,
            PeriodsSubscribedAhead = 2,
            ResponseType = 0,
            HasEverBeenMember = true,
            IsVip = true,
            PastClubDays = 0,
            PastVipDays = 0,
            MinutesTillExpire = minutesLeft,
            MinutesSinceModified = (int)(DateTime.Now - lastMod).TotalMinutes
        });

        client.Player.State.LastSubscriptionModification = DateTime.Now;
    }
}