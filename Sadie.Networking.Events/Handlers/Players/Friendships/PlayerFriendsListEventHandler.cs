using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;

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