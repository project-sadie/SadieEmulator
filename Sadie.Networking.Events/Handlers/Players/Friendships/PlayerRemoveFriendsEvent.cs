using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Friendships;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerRemoveFriendsEvent(
    PlayerRemoveFriendsParser parser,
    IPlayerRepository playerRepository,
    IPlayerFriendshipRepository friendshipRepository)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        var playerId = client.Player.Data.Id;
        
        for (var i = 0; i < parser.Amount; i++)
        {
            var currentId = parser.Ids[i];
            
            if (playerRepository.TryGetPlayerById(currentId, out var target) && target != null)
            {
                target.Data.FriendshipComponent.RemoveFriend(playerId);
                await target.NetworkObject.WriteToStreamAsync(new PlayerRemoveFriendsWriter([playerId]).GetAllBytes());
            }

            await friendshipRepository.DeleteFriendshipAsync(playerId, currentId);
        }

        client.Player.Data.FriendshipComponent.RemoveFriends(parser.Ids);
        await client.WriteToStreamAsync(new PlayerRemoveFriendsWriter(parser.Ids).GetAllBytes());
    }
}