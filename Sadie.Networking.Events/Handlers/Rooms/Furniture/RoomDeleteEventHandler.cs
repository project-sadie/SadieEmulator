using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomDelete)]
public class RoomDeleteEventHandler(
    RoomRepository roomRepository,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public required int RoomId { get; init; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var room = await roomRepository.TryLoadRoomByIdAsync(RoomId);

        if (room == null || room.OwnerId != client.Player.Id)
        {
            return;
        }

        // TODO; Return items to users inventory?

        dbContext.RoomFurnitureItems.RemoveRange(room.FurnitureItems);
        dbContext.RoomChatMessages.RemoveRange(room.ChatMessages);

        if (room.ChatSettings != null)
        {
            dbContext.RoomChatSettings.Remove(room.ChatSettings);
        }

        dbContext.RoomPaintSettings.Remove(room.PaintSettings);
        dbContext.RoomSettings.Remove(room.Settings);
        dbContext.RoomLayouts.Remove(room.Layout);

        if (room.DimmerSettings != null)
        {
            dbContext.RoomDimmerSettings.Remove(room.DimmerSettings);
        }

        dbContext
            .RoomDimmerPresets
            .RemoveRange(dbContext.RoomDimmerPresets.Where(x => x.RoomId == room.Id));

        dbContext
            .PlayerRoomVisits
            .RemoveRange(dbContext.PlayerRoomVisits.Where(x => x.RoomId == RoomId));

        dbContext
            .PlayerRoomLikes
            .RemoveRange(dbContext.PlayerRoomLikes.Where(x => x.RoomId == RoomId));

        await dbContext.SaveChangesAsync();
        
        dbContext.Rooms.Remove(room);
        await dbContext.SaveChangesAsync();
                
        foreach (var roomUser in room.UserRepository.GetAll())
        {
            await room.UserRepository.TryRemoveAsync(roomUser.Id, true);
        }
    }
}