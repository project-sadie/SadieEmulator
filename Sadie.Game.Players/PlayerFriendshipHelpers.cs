using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Packets;

namespace Sadie.Game.Players;

public class PlayerFriendshipHelpers
{
    public static async Task SendFriendUpdatesToPlayerAsync(
        PlayerLogic player, 
        List<PlayerFriendshipUpdate> updates)
    {
        await player.NetworkObject!.WriteToStreamAsync(new PlayerUpdateFriendWriter
        {
            Updates = updates
        });
    }
}