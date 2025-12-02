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
using Sadie.Networking.Packets.Writers.Rooms;
using Sadie.Networking.Packets.Writers.Rooms.Rights;

namespace Sadie.Networking.Events.Handlers.Rooms.Rights;

[PacketId(EventHandlerId.RoomGiveUserRights)]
public class RoomGiveUserRightsEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomRepository roomRepository,
    IMapper mapper) : INetworkPacketEventHandler
{
    public int PlayerId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var playerId = PlayerId;
        var player = client.Player;
        
        var room = roomRepository.TryGetRoomById(player.State.CurrentRoomId);

        if (room == null)
        {
            return;
        }

        if (room.Room.PlayerRights.FirstOrDefault(x => x.PlayerId == playerId) != null)
        {
            return;
        }

        await room.BroadcastDataAsync(new RoomGiveUserRightsWriter 
        {
            RoomId = room.Room.Id,
            PlayerId = playerId,
            PlayerUsername = player.Player.Username
        });

        if (room.UserRepository.TryGetById(playerId, out var targetRoomUser))
        {
            targetRoomUser!.ControllerLevel = RoomControllerLevel.Rights;
            targetRoomUser.ApplyFlatCtrlStatus();
            
            await targetRoomUser.NetworkObject.WriteToStreamAsync(new RoomRightsWriter
            {
                ControllerLevel = (int) targetRoomUser.ControllerLevel
            });
        }
        
        var roomPlayerRight = new RoomPlayerRightDto
        {
            RoomId = room.Room.Id,
            PlayerId = playerId,
            CreatedAt = DateTime.Now
        };
        
        room.Room.PlayerRights.Add(roomPlayerRight);
        
        var entity = mapper.Map<RoomPlayerRight>(roomPlayerRight);
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.RoomPlayerRights.Add(entity);
        await dbContext.SaveChangesAsync();
    }
}