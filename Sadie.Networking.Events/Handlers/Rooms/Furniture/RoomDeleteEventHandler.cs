using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomDelete)]
public class RoomDeleteEventHandler(
    RoomRepository roomRepository,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public required int RoomId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var room = await roomRepository.TryLoadRoomByIdAsync(RoomId);

        if (room == null || room.OwnerId != client.Player.Id)
        {
            return;
        }

        // TODO; Return items to users inventory?

        if (!roomRepository.TryRemove(RoomId, out _))
        {
            return;
        }

        dbContext.Entry<Room>(room).State = EntityState.Deleted;
        await dbContext.SaveChangesAsync();
                
        foreach (var roomUser in room.UserRepository.GetAll())
        {
            await room.UserRepository.TryRemoveAsync(roomUser.Id, true);
        }

        await dbContext.Database.ExecuteSqlRawAsync("UPDATE player_data SET home_room_id = 0 WHERE home_room_id = {0}}",
            RoomId);
    }
}