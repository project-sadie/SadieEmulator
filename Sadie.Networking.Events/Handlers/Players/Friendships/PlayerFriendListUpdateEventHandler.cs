using Sadie.API.Game.Players;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerId.PlayerFriendListUpdate)]
public class PlayerFriendListUpdateEventHandler(
    IPlayerRepository playerRepository)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await PlayerHelpers.SendPlayerFriendListUpdate(
            client.Player!, 
            playerRepository);
    }
}