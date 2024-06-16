using Sadie.Game.Players.Packets;

namespace Sadie.Game.Players;

public class PlayerHelpersDirty
{
    public static PlayerSubscriptionWriter? GetSubscriptionWriterAsync(PlayerLogic player, string name)
    {
        var playerSub = player.Subscriptions.FirstOrDefault(x => x.Subscription.Name == name);
        
        if (playerSub?.Subscription == null)
        {
            return null;
        }
        
        var tillExpire = playerSub.ExpiresAt - playerSub.CreatedAt;
        var daysLeft = (int) tillExpire.TotalDays;
        var minutesLeft = (int) tillExpire.TotalMinutes;
        var lastMod = player.State.LastSubscriptionModification;

        return new PlayerSubscriptionWriter
        {
            Name = playerSub.Subscription.Name!.ToLower(),
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
        };
    }
}