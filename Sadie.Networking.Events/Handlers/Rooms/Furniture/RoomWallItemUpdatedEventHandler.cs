using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Furniture;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

public class RoomWallItemUpdatedEventHandler(
    SadieContext dbContext,
    RoomWallItemUpdatedEventParser eventParser,
    IRoomRepository roomRepository)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomWallItemUpdated;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }
        
        var (found, room) = roomRepository.TryGetRoomById(client.Player.Data.CurrentRoomId);
        
        if (!found || room == null || !client.RoomUser.HasRights())
        {
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.MissingRights);
            return;
        }

        var roomFurnitureItem = room.FurnitureItems.FirstOrDefault(x => x.Id == eventParser.ItemId);

        if (roomFurnitureItem == null)
        {
            return;
        }

        var wallPosition = eventParser.WallPosition;

        if (string.IsNullOrEmpty(wallPosition))
        {
            return;
        }

        roomFurnitureItem.WallPosition = wallPosition;

        await dbContext.SaveChangesAsync();
        
        await room.UserRepository.BroadcastDataAsync(new RoomWallFurnitureItemUpdatedWriter(roomFurnitureItem).GetAllBytes());
    }
}