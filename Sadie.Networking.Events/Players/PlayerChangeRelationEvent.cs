using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Packets;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Players;

public class PlayerChangeRelationEvent(IPlayerRepository playerRepository, IRoomRepository roomRepository)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerId = reader.ReadInteger();
        var relationId = reader.ReadInteger();
        
        var friendshipComponent = client.Player.Data.FriendshipComponent;
        
        var friendship = friendshipComponent.
            Friendships.
            FirstOrDefault(x => x.OriginId == playerId || x.TargetId == playerId);

        if (friendship != null)
        {
            friendshipComponent.UpdateRelation(playerId, (PlayerFriendshipType) relationId);
            // TODO: Persist the update
            
            var isOnline = playerRepository.TryGetPlayerById(playerId, out var onlineFriend) && onlineFriend != null;
            var inRoom = isOnline && onlineFriend != null && onlineFriend.Data.CurrentRoomId != 0;

            var updateFriendWriter = new PlayerUpdateFriendWriter(
                    0, 
                    1, 
                    0, 
                    friendship, 
                    isOnline, 
                    inRoom, 
                    0, 
                    "", 
                    "", 
                    false, 
                    false, 
                    false)
                    .GetAllBytes();
            
            await client.WriteToStreamAsync(updateFriendWriter);
        }
    }
}