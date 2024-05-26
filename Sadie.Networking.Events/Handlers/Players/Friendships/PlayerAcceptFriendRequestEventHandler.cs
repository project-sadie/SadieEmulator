using Sadie.Database;
using Sadie.Game.Players;
using Sadie.Game.Players.Packets;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
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
    public int Amount { get; set; }
    public List<int> Ids { get; } = [];
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        foreach (var originId in Ids)
        {
            await AcceptAsync(client, originId);
        }
    }

    private async Task AcceptAsync(INetworkClient client, int originId)
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
        await dbContext.SaveChangesAsync();

        var origin = playerRepository.GetPlayerLogicById(originId);
        
        if (origin != null)
        {
            var targetRequest = origin.
                OutgoingFriendships.
                FirstOrDefault(x => x.TargetPlayerId == playerId);

            if (targetRequest != null)
            {
                const bool isOnline = true;
                var inRoom = origin.CurrentRoomId != 0;

                var relationship = origin
                        .Relationships
                        .FirstOrDefault(x =>
                            x.TargetPlayerId == targetRequest.OriginPlayerId || x.TargetPlayerId == targetRequest.TargetPlayerId);

                var updateFriendWriter = new PlayerUpdateFriendWriter
                {
                    Unknown1 = 0,
                    Unknown2 = 1,
                    Unknown3 = 0,
                    Friendship = targetRequest,
                    IsOnline = isOnline,
                    CanFollow = inRoom,
                    CategoryId = 0,
                    RealName = "",
                    LastAccess = "",
                    PersistedMessageUser = false,
                    VipMember = false,
                    PocketUser = false,
                    RelationshipType = (int)(relationship?.TypeId ?? PlayerRelationshipType.None)
                };

                await origin.NetworkObject.WriteToStreamAsync(updateFriendWriter);    
            }
        }

        var targetOnline = origin != null;
        var targetInRoom = false;

        if (targetOnline && origin != null)
        {
            var lastRoom = roomRepository.TryGetRoomById(origin.CurrentRoomId);

            if (lastRoom != null && lastRoom.UserRepository.TryGet(origin.Id, out _))
            {
                targetInRoom = true;
            }
        }
                
        var targetRelationship = targetOnline
            ? origin!
                .Relationships
                .FirstOrDefault(x => x.TargetPlayerId == request.OriginPlayerId || x.TargetPlayerId == request.TargetPlayerId) : null;
        
        var updateFriendWriter2 = new PlayerUpdateFriendWriter
        {
            Unknown1 = 0,
            Unknown2 = 1,
            Unknown3 = 0,
            Friendship = request,
            IsOnline = targetOnline,
            CanFollow = targetInRoom,
            CategoryId = 0,
            RealName = "",
            LastAccess = "",
            PersistedMessageUser = false,
            VipMember = false,
            PocketUser = false,
            RelationshipType = (int)(targetRelationship?.TypeId ?? PlayerRelationshipType.None)
        };
        
        await client.WriteToStreamAsync(updateFriendWriter2);
    }
}