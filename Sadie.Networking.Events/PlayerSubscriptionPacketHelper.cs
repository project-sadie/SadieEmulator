using Sadie.API.Interfaces.Game.Players;
using Sadie.Networking.Writers.Players.Subscriptions;

namespace Sadie.Networking.Events;

public static class PlayerSubscriptionPacketHelper
{
    public static async Task SendAsync(IPlayerLogic player)
    {
        foreach (var playerSub in player.Player.Subscriptions)
        {
            var tillExpire = playerSub.ExpiresAt - playerSub.CreatedAt;

            await player.NetworkObject.WriteToStreamAsync(new PlayerSubscriptionWriter
            {
                Name = playerSub.Subscription.Name.ToLower(),
                DaysLeft = (int)tillExpire.TotalDays,
                MinutesTillExpire = (int)tillExpire.TotalMinutes,
                MinutesSinceModified = (int)(DateTime.Now - player.State.LastSubscriptionModification).TotalMinutes,
                MemberPeriods = 0,
                PeriodsSubscribedAhead = 0,
                ResponseType = 1,
                HasEverBeenMember = true,
                IsVip = true,
                PastClubDays = 0,
                PastVipDays = 0
            });

            player.State.LastSubscriptionModification = DateTime.Now;
        }
    }
}