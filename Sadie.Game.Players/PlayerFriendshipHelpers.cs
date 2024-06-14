using Sadie.Game.Players.Packets;

namespace Sadie.Game.Players;

public class PlayerFriendshipHelpers
{
    public static async Task SendFriendUpdatesToPlayerAsync(
        PlayerLogic player, 
        List<FriendshipUpdate> updates)
    {
        await player.NetworkObject!.WriteToStreamAsync(new PlayerUpdateFriendWriter
        {
            Updates = updates
        });
    }
}