using Sadie.Game.Players;
using Sadie.Game.Players.Packets;
using Sadie.Game.Players.Relationships;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerChangeRelationshipEventHandler(
    PlayerChangeRelationshipEventParser eventParser,
    IPlayerRepository playerRepository, 
    IPlayerDao playerDao)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerChangeRelationship;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var playerId = eventParser.PlayerId;
        var relationId = eventParser.RelationId;
        
        var friendshipComponent = client.Player.Data.FriendshipComponent;
        
        var friendship = friendshipComponent.
            Friendships.
            FirstOrDefault(x => x.OriginId == playerId || x.TargetId == playerId);

        if (friendship == null)
        {
            return;
        }

        if (relationId == 0)
        {
            var relationship = client.Player.Data.Relationships.FirstOrDefault(x => x.TargetPlayerId == playerId);

            if (relationship != null)
            {
                await playerDao.DeleteRelationshipAsync(relationship.Id);
                client.Player.Data.Relationships.Remove(relationship);
            }
        }
        else
        {
            var relationship = client.Player.Data.Relationships.FirstOrDefault(x => x.TargetPlayerId == playerId);
        
            if (relationship == null)
            {
                relationship = await playerDao.CreateRelationshipAsync(client.Player.Data.Id, playerId, (PlayerRelationshipType)relationId);
                client.Player.Data.Relationships.Add(relationship);
            }
            else
            {
                relationship.Type = (PlayerRelationshipType)relationId;
                await playerDao.UpdateRelationshipTypeAsync(relationship.Id, (PlayerRelationshipType) relationId);
            }
        }
        
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
                false,
                (PlayerRelationshipType) relationId)
            .GetAllBytes();
            
        await client.WriteToStreamAsync(updateFriendWriter);
    }
}