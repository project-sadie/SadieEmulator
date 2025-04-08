using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Enums.Game.Rooms.Users;
using Sadie.Game.Rooms.Packets.Writers.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserRespect)]
public class RoomUserRespectEventHandler(
    IPlayerRepository playerRepository,
    IRoomRepository roomRepository,
    IDbContextFactory<SadieContext> dbContextFactory)
    : INetworkPacketEventHandler
{
    public int TargetId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        var player = client.Player!;
        var playerData = player.Data;
        var lastRoom = player.State.CurrentRoomId;
        var targetPlayer = playerRepository.GetPlayerLogicById(TargetId);
        
        if (playerData.RespectPoints < 1 || 
            player.Id == TargetId || 
            targetPlayer == null || 
            targetPlayer.State.CurrentRoomId != 0 && lastRoom != targetPlayer.State.CurrentRoomId)
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

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(playerData).Property(x => x.RespectPoints).IsModified = true;
        await dbContext.SaveChangesAsync();

        await room.UserRepository.BroadcastDataAsync(new RoomUserRespectWriter
        {
            UserId = TargetId,
            TotalRespects = targetPlayer.Respects.Count
        });
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserActionWriter
        {
            UserId = roomUser.Player.Id,
            Action = (int) RoomUserAction.ThumbsUp
        });
    }
}