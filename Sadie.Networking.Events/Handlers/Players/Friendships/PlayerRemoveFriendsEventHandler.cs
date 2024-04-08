using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Friendships;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerRemoveFriendsEventHandler(
    PlayerRemoveFriendsEventParser eventParser,
    PlayerRepository playerRepository,
    IPlayerFriendshipRepository friendshipRepository)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerRemoveFriends;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var playerId = client.Player.Data.Id;
        
        for (var i = 0; i < eventParser.Amount; i++)
        {
            var currentId = eventParser.Ids[i];
            
            if (playerRepository.TryGetPlayerById(currentId, out var target) && target != null)
            {
                target.FriendshipComponent.RemoveFriend(playerId);
                await target.NetworkObject.WriteToStreamAsync(new PlayerRemoveFriendsWriter([playerId]).GetAllBytes());
            }

            await friendshipRepository.DeleteFriendshipAsync(playerId, currentId);
        }

        client.Player.FriendshipComponent.RemoveFriends(eventParser.Ids);
        await client.WriteToStreamAsync(new PlayerRemoveFriendsWriter(eventParser.Ids).GetAllBytes());
    }
}