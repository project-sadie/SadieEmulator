using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Enums.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Dtos;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerId.PlayerAcceptFriendRequest)]
public class PlayerAcceptFriendRequestEventHandler(
    IPlayerRepository playerRepository,
    IRoomRepository roomRepository,
    SadieContext dbContext,
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
                Relation = targetRelationship?.TypeId ?? PlayerRelationshipType.None
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
                    Relation = relationship?.TypeId ?? PlayerRelationshipType.None
                }
            ]);
        }
    }
}