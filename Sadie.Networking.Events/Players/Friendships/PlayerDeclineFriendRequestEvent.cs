using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Players.Friendships;

public class PlayerDeclineFriendRequestEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerFriendshipRepository _friendshipRepository;

    public PlayerDeclineFriendRequestEvent(IPlayerRepository playerRepository, IPlayerFriendshipRepository friendshipRepository)
    {
        _playerRepository = playerRepository;
        _friendshipRepository = friendshipRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var declineAll = reader.ReadBool();
        const int limit = 100;

        if (declineAll)
        {
            await _friendshipRepository.DeclineAllFriendRequestsAsync(client.Player!.Id);
        }
        else
        {
            var amount = reader.ReadInt();

            for (var i = 0; i < amount && i < limit; i++)
            {
                var originId = reader.ReadInt();
                var targetId = client.Player!.Id;

                await _friendshipRepository.DeclineFriendRequestAsync(originId, targetId);
                client.Player.FriendshipComponent.DeclineIncomingRequest(originId);

                if (_playerRepository.TryGetPlayerById(originId, out var origin) && origin != null)
                {
                    origin.FriendshipComponent.OutgoingRequestDeclined(targetId);
                }
            }
        }
    }
}