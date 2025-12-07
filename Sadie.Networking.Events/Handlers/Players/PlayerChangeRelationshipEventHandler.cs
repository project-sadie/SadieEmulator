using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Players;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Networking.Events.Dtos;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerChangeRelationship)]
public class PlayerChangeRelationshipEventHandler(
    IPlayerRepository playerRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper)
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

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        if (relationId == 0)
        {
            var relationship = client.Player.Player.Relationships.FirstOrDefault(x => x.TargetPlayerId == playerId);

            if (relationship != null)
            {
                client.Player.Player.Relationships.Remove(relationship);
                dbContext.Entry(relationship).State = EntityState.Deleted;
                await dbContext.SaveChangesAsync();
            }
        }
        else
        {
            var relationship = client.Player.Player.Relationships.FirstOrDefault(x => x.TargetPlayerId == playerId);
        
            if (relationship == null)
            {
                relationship = new PlayerRelationshipDto
                {
                    OriginPlayerId = client.Player.Player.Id,
                    TargetPlayerId = playerId,
                    TargetPlayer = await playerRepository.GetPlayerByIdAsync(playerId),
                    TypeId = relationId
                };
                
                client.Player.Player.Relationships.Add(relationship);
                
                dbContext.Entry(relationship).State = EntityState.Added;
                dbContext.Attach(relationship.TargetPlayer!).State = EntityState.Unchanged;
                
                await dbContext.SaveChangesAsync();
            }
            else
            {
                relationship.TypeId = relationId;
                dbContext.Entry(relationship).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
            }
        }

        var onlineFriend = playerRepository.GetPlayerLogicById(playerId);
        var isOnline = onlineFriend != null;
        var inRoom = isOnline && onlineFriend!.State.CurrentRoomId != 0;

        var friend = isOnline ? 
            mapper.Map<PlayerDto>(onlineFriend) : 
            await playerRepository.GetPlayerByIdAsync(playerId);
        
        var newFriendData = new FriendData
        {
            Motto = friend.AvatarData.Motto,
            Gender = PlayerAvatarGender.Male,
            Username = friend.Username,
            FigureCode = friend.AvatarData.FigureCode
        };
        
        var updateFriendWriter = new PlayerUpdateFriendWriter
        {
            Updates =
            [
                new PlayerFriendshipUpdate
                {
                    Type = 0,
                    Friend = newFriendData,
                    FriendOnline = isOnline,
                    FriendInRoom = inRoom,
                    Relation = (Core.Enums.Game.Players.PlayerRelationshipType) relationId
                }
            ]
        };
            
        await client.WriteToStreamAsync(updateFriendWriter);
    }
}