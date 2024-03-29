using Sadie.Game.Rooms;
using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Furniture;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

public class RoomWallFurnitureItemUpdatedEvent(
    RoomWallFurnitureItemUpdatedParser parser,
    IRoomRepository roomRepository,
    IRoomFurnitureItemDao roomFurnitureItemDao)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

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
            await PacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.MissingRights);
            return;
        }

        var roomFurnitureItem = room.FurnitureItemRepository.Items.FirstOrDefault(x => x.Id == parser.ItemId);

        if (roomFurnitureItem == null)
        {
            return;
        }

        var wallPosition = parser.WallPosition;

        if (string.IsNullOrEmpty(wallPosition))
        {
            return;
        }

        roomFurnitureItem.WallPosition = wallPosition;
        
        await roomFurnitureItemDao.UpdateItemAsync(roomFurnitureItem);
        await room.UserRepository.BroadcastDataAsync(new RoomWallFurnitureItemUpdatedWriter(roomFurnitureItem).GetAllBytes());
    }
}