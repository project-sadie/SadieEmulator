using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Networking.Events.Dtos;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerId.PlayerAcceptFriendRequest)]
public class PlayerAcceptFriendRequestEventHandler(
    IPlayerRepository playerRepository,
    IRoomRepository roomRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IPlayerHelperService playerHelperService)
    : INetworkPacketEventHandler
{
    public List<int> Ids { get; set; } = [];
    
    public async Task HandleAsync(INetworkClient client)
    {
        foreach (var originId in Ids)
        {
            await AcceptRequestAsync(client, originId);
        }
    }

    private async Task AcceptRequestAsync(INetworkClient client, int originId)
    {
        var player = client.Player;
        var playerId = player.Id;
        
        var request = player
            .IncomingFriendships
            .FirstOrDefault(x => x.OriginPlayerId == originId && x.Status == PlayerFriendshipStatus.Pending);

        if (request == null || request.TargetPlayerId != playerId)
        {
            return;
        }

        request.Status = PlayerFriendshipStatus.Accepted;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(request).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
        
        var targetPlayer = playerRepository.GetPlayerLogicById(originId);
        var targetOnline = targetPlayer != null;
        var targetInRoom = targetPlayer != null && targetPlayer.State.CurrentRoomId != 0;

        var targetRelationship = targetOnline
            ? targetPlayer!
                .Relationships
                .FirstOrDefault(x => x.TargetPlayerId == request.OriginPlayerId || x.TargetPlayerId == request.TargetPlayerId) : null;

        await playerHelperService.SendFriendUpdatesToPlayerAsync(client.Player, [
            new PlayerFriendshipUpdate
            {
                Type = 0,
                Friend = new FriendData
                {
                    Username = targetPlayer.Username,
                    FigureCode = targetPlayer.AvatarData.FigureCode,
                    Motto = targetPlayer.AvatarData.Motto,
                    Gender = targetPlayer.AvatarData.Gender
                },
                FriendOnline = targetOnline,
                FriendInRoom = targetInRoom,
                Relation = (PlayerRelationshipType?)targetRelationship?.TypeId ?? PlayerRelationshipType.None
            }
        ]);

        if (targetOnline)
        {
            var targetRequest = targetPlayer.
                OutgoingFriendships.
                FirstOrDefault(x => x.TargetPlayerId == playerId);

            if (targetRequest == null)
            {
                return;
            }
            
            var relationship = targetPlayer
                .Relationships
                .FirstOrDefault(x =>
                    x.TargetPlayerId == targetRequest.OriginPlayerId || x.TargetPlayerId == targetRequest.TargetPlayerId);

            await playerHelperService.SendFriendUpdatesToPlayerAsync(targetPlayer, [
                new PlayerFriendshipUpdate
                {
                    Type = 0,
                    Friend = new FriendData
                    {
                        Username = player.Username,
                        FigureCode = player.AvatarData.FigureCode,
                        Motto = player.AvatarData.Motto,
                        Gender = player.AvatarData.Gender
                    },
                    FriendOnline = true,
                    FriendInRoom = player.State.CurrentRoomId != 0,
                    Relation = (PlayerRelationshipType?)relationship?.TypeId ?? PlayerRelationshipType.None
                }
            ]);
        }
    }
}