using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerIds.PlayerAcceptFriendRequest)]
public class PlayerAcceptFriendRequestEventHandler(
    PlayerRepository playerRepository,
    RoomRepository roomRepository,
    SadieContext dbContext)
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

        await PlayerFriendshipHelpers.SendFriendUpdatesToPlayerAsync(client.Player, [
            new PlayerFriendshipUpdate
            {
                Type = 0,
                Friend = targetPlayer,
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

            await PlayerFriendshipHelpers.SendFriendUpdatesToPlayerAsync(targetPlayer, [
                new PlayerFriendshipUpdate
                {
                    Type = 0,
                    Friend = player,
                    FriendOnline = true,
                    FriendInRoom = player.State.CurrentRoomId != 0,
                    Relation = relationship?.TypeId ?? PlayerRelationshipType.None
                }
            ]);
        }
    }
}