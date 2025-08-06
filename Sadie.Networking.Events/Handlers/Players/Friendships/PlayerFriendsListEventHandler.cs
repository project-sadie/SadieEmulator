using Sadie.API.Game.Players;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerId.PlayerFriendsList)]
public class PlayerFriendsListEventHandler(
    IPlayerRepository playerRepository,
    IPlayerHelperService playerHelperService)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await playerHelperService.SendPlayerFriendListUpdate(
            client.Player!, 
            playerRepository);
    }
}