using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerDeclineFriendRequestEvent(
    IPlayerRepository playerRepository,
    IPlayerFriendshipRepository friendshipRepository)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;
        var playerData = player.Data;
        var playerId = playerData.Id;
        
        var declineAll = reader.ReadBool();
        const int limit = 100;

        if (declineAll)
        {
            await friendshipRepository.DeclineAllFriendRequestsAsync(playerId);
        }
        else
        {
            var amount = reader.ReadInteger();

            for (var i = 0; i < amount && i < limit; i++)
            {
                var originId = reader.ReadInteger();
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