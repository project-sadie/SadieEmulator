using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerIds.PlayerFriendsList)]
public class PlayerFriendsListEventHandler(
    PlayerRepository playerRepository)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await PlayerHelpersToClean.SendPlayerFriendListUpdate(
            client.Player!, 
            playerRepository);
    }
}