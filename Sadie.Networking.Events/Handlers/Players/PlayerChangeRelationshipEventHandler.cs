using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Game.Players.Packets;
using Sadie.Game.Players.Relationships;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerChangeRelationshipEventHandler(
    PlayerChangeRelationshipEventParser eventParser,
    PlayerRepository playerRepository,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerChangeRelationship;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var playerId = eventParser.PlayerId;
        var relationId = eventParser.RelationId;
        
        var friendshipComponent = client.Player.FriendshipComponent;
        
        var friendship = friendshipComponent.
            Friendships.
            FirstOrDefault(x => x.OriginId == playerId || x.TargetId == playerId);

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
                    OriginPlayerId = client.Player.Data.Id,
                    TargetPlayerId = playerId,
                    Type = (PlayerRelationshipType)relationId
                };
                
                client.Player.Relationships.Add(relationship);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                relationship.Type = (PlayerRelationshipType)relationId;
                await dbContext.SaveChangesAsync();
            }
        }
        
        var isOnline = playerRepository.TryGetPlayerById(playerId, out var onlineFriend) && onlineFriend != null;
        var inRoom = isOnline && onlineFriend != null && onlineFriend.CurrentRoomId != 0;
        
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
                false,
                (PlayerRelationshipType) relationId)
            .GetAllBytes();
            
        await client.WriteToStreamAsync(updateFriendWriter);
    }
}