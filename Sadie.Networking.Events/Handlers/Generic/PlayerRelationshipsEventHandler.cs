using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Generic;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Generic;

namespace Sadie.Networking.Events.Handlers.Generic;

public class PlayerRelationshipsEventHandler(
    PlayerRelationshipsEventParser eventParser,
    PlayerRepository playerRepository,
    IPlayerFriendshipRepository friendshipRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerRelationships;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var playerId = eventParser.PlayerId;
        var isOnline = playerRepository.TryGetPlayerById(playerId, out var player);
        
        var relationships = isOnline ? 
            player!.Relationships : 
            await playerRelationshipDao.GetRelationshipsAsync(playerId);

        var playerFriends = isOnline
            ? player!.FriendshipComponent.Friendships
            : await friendshipRepository.GetFriendsForPlayerAsync(playerId);
        
        await client.WriteToStreamAsync(new PlayerRelationshipsWriter(playerId, 
            relationships, 
            playerFriends).GetAllBytes());
    }
}