using Sadie.API.Game.Players;
using Sadie.Database.Models.Players;
using Sadie.Enums.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Packets.Writers;

namespace Sadie.Game.Players;

public class PlayerHelpers
{
    public static async Task SendFriendUpdatesToPlayerAsync(
        IPlayerLogic player, 
        List<PlayerFriendshipUpdate> updates)
    {
        await player.NetworkObject!.WriteToStreamAsync(new PlayerUpdateFriendWriter
        {
            Updates = updates
        });
    }

    public static async Task SendPlayerFriendListUpdate(
        PlayerLogic player, 
        IPlayerRepository playerRepository)
    {
        var friends = player
            .GetMergedFriendships()
            .Where(x => x.Status == PlayerFriendshipStatus.Accepted)
            .ToList();
        
        var pages = friends.Count / 500 + 1;
        
        for (var i = 0; i < pages; i++)
        {
            var batch = friends.Skip(i * 500).
                Take(500).
                ToList();
            
            await player.NetworkObject!.WriteToStreamAsync(new PlayerFriendsListWriter
            {
                Pages = pages,
                Index = i,
                PlayerId = player.Id,
                Friends = batch,
                PlayerRepository = playerRepository,
                Relationships = player.Relationships
            });
        }
    }
    
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

    public static async Task UpdatePlayerStatusForFriendsAsync(
        Player player, 
        IEnumerable<PlayerFriendship> friendships, 
        bool isOnline, 
        bool inRoom,
        IPlayerRepository playerRepository)
    {
        var update = new PlayerFriendshipUpdate
        {
            Type = 0,
            Friend = player,
            FriendOnline = isOnline,
            FriendInRoom = inRoom,
            Relation = PlayerRelationshipType.None
        };
        
        foreach (var friend in friendships)
        {
            var targetId = friend.OriginPlayerId == player.Id ? 
                friend.TargetPlayerId : 
                friend.OriginPlayerId;

            var targetPlayer = playerRepository.GetPlayerLogicById(targetId);

            if (targetPlayer != null)
            {
                await SendFriendUpdatesToPlayerAsync(targetPlayer, [update]);
            }
        }
    }
}