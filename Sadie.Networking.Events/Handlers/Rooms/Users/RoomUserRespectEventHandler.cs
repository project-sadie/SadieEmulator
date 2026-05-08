using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Players;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Rooms.Users;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Db.Models.Players;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserRespect)]
public class RoomUserRespectEventHandler(
    IPlayerRepository playerRepository,
    IRoomRepository roomRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper)
    : INetworkPacketEventHandler
{
    public int TargetId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!RoomContextResolver.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        var player = client.Player!;
        var playerData = player.Player.Data;
        var lastRoom = player.State.CurrentRoomId;
        var targetPlayer = playerRepository.GetPlayerLogicById(TargetId);
        
        if (playerData.RespectPoints < 1 || 
            player.Player.Id == TargetId || 
            targetPlayer == null || 
            targetPlayer.State.CurrentRoomId != 0 && lastRoom != targetPlayer.State.CurrentRoomId)
        {
            return;
        }

        var respect = new PlayerRespectDto
        {
            OriginPlayerId = player.Player.Id,
            TargetPlayerId = targetPlayer.Player.Id
        };

        playerData.RespectPoints--;
        targetPlayer.Player.Respects.Add(respect);

        var respectEntity = mapper.Map<PlayerRespect>(respect);
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.PlayerRespects.Add(respectEntity);
        dbContext.Entry(playerData).Property(x => x.RespectPoints).IsModified = true;
        await dbContext.SaveChangesAsync();

        await room.BroadcastDataAsync(new RoomUserRespectWriter
        {
            UserId = TargetId,
            TotalRespects = targetPlayer.Player.Respects.Count
        });
        
        await room.BroadcastDataAsync(new RoomUserActionWriter
        {
            UserId = roomUser.Player.Player.Id,
            Action = (int) RoomUserAction.ThumbsUp
        });
    }
}