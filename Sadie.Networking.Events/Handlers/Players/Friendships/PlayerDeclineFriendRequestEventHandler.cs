using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Friendships;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerDeclineFriendRequestEventHandler(
    PlayerDeclineFriendRequestEventParser eventParser,
    IPlayerRepository playerRepository,
    IPlayerFriendshipRepository friendshipRepository)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var player = client.Player;
        var playerData = player.Data;
        var playerId = playerData.Id;

        if (eventParser.DeclineAll)
        {
            await friendshipRepository.DeclineAllFriendRequestsAsync(playerId);
        }
        else
        {
            foreach (var originId in eventParser.Ids) 
            {
                var targetId = playerId;

                await friendshipRepository.DeclineFriendRequestAsync(originId, targetId);
                playerData.FriendshipComponent.DeclineIncomingRequest(originId);

                if (playerRepository.TryGetPlayerById(originId, out var origin) && origin != null)
                {
                    origin.Data.FriendshipComponent.OutgoingRequestDeclined(targetId);
                }
            }
        }
    }
}