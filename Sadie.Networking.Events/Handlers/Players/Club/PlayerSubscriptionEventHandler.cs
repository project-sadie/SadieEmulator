using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players.Club;

[PacketId(EventHandlerIds.PlayerSubscription)]
public class PlayerSubscriptionEventHandler : INetworkPacketEventHandler
{
    public string? Name { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var playerSub = client.Player?.Subscriptions.FirstOrDefault(x => x.Subscription.Name == Name);
        
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
            Name = playerSub.Subscription.Name.ToLower(),
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