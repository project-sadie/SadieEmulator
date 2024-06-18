using Sadie.API.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Packets;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Players;

public class PlayerFriendshipHelpers
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
        PlayerRepository playerRepository)
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
            
            await player.NetworkObject.WriteToStreamAsync(new PlayerFriendsListWriter
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
}