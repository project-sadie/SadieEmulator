using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Friendships;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerIds.PlayerFriendListUpdate)]
public class PlayerFriendListUpdateEventHandler(
    PlayerRepository playerRepository)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await PlayerFriendshipHelpers.SendPlayerFriendListUpdate(
            client.Player!, 
            playerRepository);
    }
}