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
public sealed class PlayerChangeRelationshipEventHandler(
    IPlayerRepository playerRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper)
    : INetworkPacketEventHandler
{
    public int PlayerId { get; set; }
    public int RelationId { get; set; }

    public async Task HandleAsync(INetworkClient client)
    {
        var targetPlayerId = PlayerId;
        var relationId = RelationId;

        var friendship = client.Player.TryGetAcceptedFriendshipFor(targetPlayerId);
        if (friendship is null)
        {
            return;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        await UpdateRelationshipAsync(
            client,
            dbContext,
            targetPlayerId,
            relationId
        );

        await SendFriendUpdateAsync(
            client,
            playerRepository,
            mapper,
            targetPlayerId,
            relationId
        );
    }

    private static async Task UpdateRelationshipAsync(
        INetworkClient client,
        SadieDbContext dbContext,
        int targetPlayerId,
        int relationId)
    {
        var originPlayer = client.Player.Player;

        var relationship = originPlayer.OriginRelationships
            .FirstOrDefault(x => x.TargetPlayerId == targetPlayerId);

        if (relationId == 0)
        {
            if (relationship is null)
            {
                return;
            }

            originPlayer.OriginRelationships.Remove(relationship);
            dbContext.Remove(relationship);
            await dbContext.SaveChangesAsync();
            return;
        }

        if (relationship is null)
        {
            relationship = new PlayerRelationshipDto
            {
                OriginPlayerId = originPlayer.Id,
                TargetPlayerId = targetPlayerId,
                TypeId = relationId
            };

            originPlayer.OriginRelationships.Add(relationship);
            dbContext.Add(relationship);
            await dbContext.SaveChangesAsync();
            return;
        }

        relationship.TypeId = relationId;
        await dbContext.SaveChangesAsync();
    }

    private static async Task SendFriendUpdateAsync(
        INetworkClient client,
        IPlayerRepository playerRepository,
        IMapper mapper,
        int targetPlayerId,
        int relationId)
    {
        var onlineFriend = playerRepository.GetPlayerLogicById(targetPlayerId);
        var isOnline = onlineFriend is not null;
        var inRoom = isOnline && onlineFriend!.State.CurrentRoomId != 0;

        var friend = isOnline
            ? mapper.Map<PlayerDto>(onlineFriend!)
            : await playerRepository.GetPlayerByIdAsync(targetPlayerId);

        var friendData = new FriendData
        {
            Username = friend.Username,
            Motto = friend.AvatarData.Motto,
            FigureCode = friend.AvatarData.FigureCode,
            Gender = PlayerAvatarGender.Male 
        };

        var writer = new PlayerUpdateFriendWriter
        {
            Updates =
            [
                new PlayerFriendshipUpdate
                {
                    Type = 0,
                    Friend = friendData,
                    FriendOnline = isOnline,
                    FriendInRoom = inRoom,
                    Relation = (PlayerRelationshipType) relationId
                }
            ]
        };

        await client.WriteToStreamAsync(writer);
    }
}