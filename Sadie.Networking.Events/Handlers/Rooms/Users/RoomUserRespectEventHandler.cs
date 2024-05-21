using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerIds.RoomUserRespect)]
public class RoomUserRespectEventHandler(
    PlayerRepository playerRepository,
    RoomRepository roomRepository,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        var player = client.Player!;
        var playerData = player.Data;
        var lastRoom = player.CurrentRoomId;
        var targetPlayer = playerRepository.GetPlayerLogicById(TargetId);
        
        if (playerData.RespectPoints < 1 || 
            player.Id == TargetId || 
            targetPlayer == null || 
            targetPlayer.CurrentRoomId != 0 && lastRoom != targetPlayer.CurrentRoomId)
        {
            return;
        }

        var respect = new PlayerRespect
        {
            OriginPlayerId = player.Id,
            TargetPlayerId = targetPlayer.Id
        };

        playerData.RespectPoints--;
        targetPlayer.Respects.Add(respect);

        await dbContext.SaveChangesAsync();

        await room.UserRepository.BroadcastDataAsync(new RoomUserRespectWriter
        {
            UserId = TargetId,
            TotalRespects = targetPlayer.Respects.Count
        });
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserActionWriter
        {
            UserId = roomUser.Id,
            Action = (int) RoomUserAction.ThumbsUp
        });
    }
}