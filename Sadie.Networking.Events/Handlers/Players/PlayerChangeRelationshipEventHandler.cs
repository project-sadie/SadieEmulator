using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Game.Players.Packets;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Shared.Unsorted;

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
            ;
            
        await client.WriteToStreamAsync(updateFriendWriter);
    }
}