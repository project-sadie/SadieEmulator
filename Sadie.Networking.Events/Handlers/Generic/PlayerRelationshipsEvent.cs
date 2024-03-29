using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Generic;

namespace Sadie.Networking.Events.Handlers.Generic;

public class PlayerRelationshipsEvent(
    IPlayerRepository playerRepository,
    IPlayerDao playerDao,
    IPlayerFriendshipRepository friendshipRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerId = reader.ReadInteger();
        var isOnline = playerRepository.TryGetPlayerById(playerId, out var player);
        
        var relationships = isOnline ? 
            player!.Data.Relationships : 
            await playerDao.GetRelationshipsAsync(playerId);

        var playerFriends = isOnline
            ? player!.Data.FriendshipComponent.Friendships
            : await friendshipRepository.GetFriendsForPlayerAsync(playerId);
        
        await client.WriteToStreamAsync(new PlayerRelationshipsWriter(playerId, 
            relationships, 
            playerFriends).GetAllBytes());
    }
}