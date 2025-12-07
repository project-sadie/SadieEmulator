using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Rooms.Rights;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Rooms;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Db.Models.Rooms.Rights;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Rights;

namespace Sadie.Networking.Events.Handlers.Rooms.Rights;

[PacketId(EventHandlerId.RoomRemoveUserRights)]
public class RoomRemoveUserRightsEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomRepository roomRepository,
    IMapper mapper) : INetworkPacketEventHandler
{
    public required List<int> Ids { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null)
        {
            return;
        }
        
        var room = roomRepository.TryGetRoomById(client.Player.State.CurrentRoomId);

        if (room == null)
        {
            return;
        }
        
        foreach (var playerId in Ids)
        {
            var right = room.Room.PlayerRights.FirstOrDefault(x => x.PlayerId == playerId);
            
            if (right == null)
            {
                continue;
            }
            
            await RemoveRoomPlayerRightAsync(playerId, room, right);
        }
    }

    private async Task RemoveRoomPlayerRightAsync(long playerId, IRoomLogic room, RoomPlayerRightDto right)
    {
        if (room.UserRepository.TryGetById((int) playerId, out var roomUser))
        {
            roomUser!.ControllerLevel = RoomControllerLevel.None;
            roomUser.ApplyFlatCtrlStatus();
            
            await roomUser.NetworkObject.WriteToStreamAsync(new RoomRightsWriter
            {
                ControllerLevel = (int) roomUser.ControllerLevel
            });
        }
        
        room.Room.PlayerRights.Remove(right);

        var rightEntity = mapper.Map<RoomPlayerRight>(right);

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.RoomPlayerRights.Remove(rightEntity);
        await dbContext.SaveChangesAsync();

        await room.BroadcastDataAsync(
            new RoomRemoveUserRightsWriter
            {
                RoomId = room.Room.Id,
                PlayerId = playerId
            });
    }
}