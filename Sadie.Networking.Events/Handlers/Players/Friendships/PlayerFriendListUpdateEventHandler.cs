using Sadie.API.Game.Players;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerId.PlayerFriendListUpdate)]
public class PlayerFriendListUpdateEventHandler(
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