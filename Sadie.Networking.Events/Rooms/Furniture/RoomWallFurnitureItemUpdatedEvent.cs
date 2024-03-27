using Sadie.Game.Rooms;
using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Rooms.Furniture;

public class RoomWallFurnitureItemUpdatedEvent(
    IRoomRepository roomRepository,
    IRoomFurnitureItemDao roomFurnitureItemDao)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }

        var itemId = reader.ReadInteger();
        
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

        var roomFurnitureItem = room.FurnitureItemRepository.Items.FirstOrDefault(x => x.Id == itemId);

        if (roomFurnitureItem == null)
        {
            return;
        }
        
        var wallPosition =  reader.ReadString();

        if (string.IsNullOrEmpty(wallPosition))
        {
            return;
        }

        roomFurnitureItem.WallPosition = wallPosition;
        
        await roomFurnitureItemDao.UpdateItemAsync(roomFurnitureItem);
        await room.UserRepository.BroadcastDataAsync(new RoomWallFurnitureItemUpdatedWriter(roomFurnitureItem).GetAllBytes());
    }
}