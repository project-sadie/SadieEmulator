using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms;

public class RoomHeightmapEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomHeightmap;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var (found, room) = roomRepository.TryGetRoomById(client.Player.Data.CurrentRoomId);
        
        if (!found || room == null)
        {
            return;
        }

        var roomLayoutData = room.LayoutData;
        var userRepository = room.UserRepository;
        var isOwner = room.OwnerId == client.Player.Data.Id;
        
        await client.WriteToStreamAsync(new RoomRelativeMapWriter(roomLayoutData).GetAllBytes());
        await client.WriteToStreamAsync(new RoomHeightMapWriter(true, -1, room.Layout.HeightMap.Replace("\r\n", "\r")).GetAllBytes());
        
        await userRepository.BroadcastDataAsync(new RoomUserDataWriter(room.UserRepository.GetAll()).GetAllBytes());
        await userRepository.BroadcastDataAsync(new RoomUserStatusWriter(room.UserRepository.GetAll()).GetAllBytes());
        
        var floorItems = room.FurnitureItems
            .Where(x => x.FurnitureItem.Type == FurnitureItemType.Floor)
            .ToList();
        
        var wallItems = room.FurnitureItems
            .Where(x => x.FurnitureItem.Type == FurnitureItemType.Wall)
            .ToList();

        var floorFurnitureOwners = 
            floorItems
                .Select(item => new { Key = item.OwnerId, Value = item.OwnerUsername })
                .Distinct()
                .ToDictionary(x => x.Key, x => x.Value);

        var wallFurnitureOwners = 
            wallItems
                .Select(item => new { Key = item.OwnerId, Value = item.OwnerUsername })
                .Distinct()
                .ToDictionary(x => x.Key, x => x.Value);

        await client.WriteToStreamAsync(new RoomFloorItemsWriter(floorItems, floorFurnitureOwners).GetAllBytes());
        await client.WriteToStreamAsync(new RoomWallItemsWriter(wallItems, wallFurnitureOwners).GetAllBytes());
        
        await userRepository.BroadcastDataAsync(new RoomForwardDataWriter(room, false, true, isOwner).GetAllBytes());
    }
}