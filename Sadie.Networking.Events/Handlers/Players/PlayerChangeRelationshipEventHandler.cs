using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Packets;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerIds.PlayerChangeRelationship)]
public class PlayerChangeRelationshipEventHandler(
    PlayerRepository playerRepository,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public int PlayerId { get; set; }
    public int RelationId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var playerId = PlayerId;
        var relationId = RelationId;

        var friendship = client.Player.TryGetAcceptedFriendshipFor(playerId);
        
        if (friendship == null)
        {
            return;
        }

        if (relationId == 0)
        {
            var relationship = client.Player.Relationships.FirstOrDefault(x => x.TargetPlayerId == playerId);

            if (relationship != null)
            {
                client.Player.Relationships.Remove(relationship);
                await dbContext.SaveChangesAsync();
            }
        }
        else
        {
            var relationship = client.Player.Relationships.FirstOrDefault(x => x.TargetPlayerId == playerId);
        
            if (relationship == null)
            {
                relationship = new PlayerRelationship
                {
                    OriginPlayerId = client.Player.Id,
                    TargetPlayerId = playerId,
                    TypeId = (PlayerRelationshipType)relationId
                };
                
                client.Player.Relationships.Add(relationship);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                relationship.TypeId = (PlayerRelationshipType)relationId;
                await dbContext.SaveChangesAsync();
            }
        }

        var onlineFriend = playerRepository.GetPlayerLogicById(playerId);
        var isOnline = onlineFriend != null;
        var inRoom = isOnline && onlineFriend!.CurrentRoomId != 0;
        
        var updateFriendWriter = new PlayerUpdateFriendWriter
        {
            Updates =
            [
                new PlayerFriendshipUpdate
                {
                    Type = 0,
                    Friend = isOnline ? onlineFriend : await playerRepository.GetPlayerByIdAsync(playerId),
                    FriendOnline = isOnline,
                    FriendInRoom = inRoom,
                    Relation = (PlayerRelationshipType)relationId
                }
            ]
        };
            
        await client.WriteToStreamAsync(updateFriendWriter);
    }
}