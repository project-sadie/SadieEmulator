using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerIds.RoomHeightmap)]
public class RoomHeightmapEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var room = roomRepository.TryGetRoomById(client.Player.State.CurrentRoomId);
        
        if (room == null)
        {
            return;
        }

        var roomTileMap = room.TileMap;
        var userRepository = room.UserRepository;
        var isOwner = room.OwnerId == client.Player.Id;
        
        await client.WriteToStreamAsync(new RoomRelativeMapWriter
        {
            TileMap = roomTileMap
        });
        
        await client.WriteToStreamAsync(new RoomHeightMapWriter
        {
            Unknown1 = true,
            WallHeight = -1,
            RelativeHeightmap = room.Layout.HeightMap.Replace("\r\n", "\r")
        });
        
        await client.WriteToStreamAsync(new RoomWallFloorSettingsWriter
        {
            HideWalls = room.Settings.HideWalls,
            WallThickness = room.Settings.WallThickness,
            FloorThickness = room.Settings.FloorThickness
        });
        
        await client.WriteToStreamAsync(new RoomUserDataWriter
        {
            Users = room.UserRepository.GetAll()
        });
        
        await client.WriteToStreamAsync(new RoomUserStatusWriter
        {
            Users = room.UserRepository.GetAll()
        });

        if (room.BotRepository.Count > 0)
        {
            await client.WriteToStreamAsync(new RoomBotDataWriter
            {
                Bots = room.BotRepository.GetAll()
            });
        
            await client.WriteToStreamAsync(new RoomBotStatusWriter
            {
                Bots = room.BotRepository.GetAll()
            });
        }
        
        var floorItems = room.FurnitureItems
            .Where(x => x.FurnitureItem.Type == FurnitureItemType.Floor)
            .ToList();
        
        var wallItems = room.FurnitureItems
            .Where(x => x.FurnitureItem.Type == FurnitureItemType.Wall)
            .ToList();

        var floorFurnitureOwners = 
            floorItems
                .Select(item => new { Key = item.PlayerFurnitureItem.PlayerId, Value = item.PlayerFurnitureItem.Player.Username })
                .Distinct()
                .ToDictionary(x => x.Key, x => x.Value);

        var wallFurnitureOwners = 
            wallItems
                .Select(item => new { Key = item.PlayerFurnitureItem.PlayerId, Value = item.PlayerFurnitureItem.Player.Username })
                .Distinct()
                .ToDictionary(x => x.Key, x => x.Value);

        await client.WriteToStreamAsync(new RoomFloorItemsWriter
        {
            FloorItems = floorItems,
            FurnitureOwners = floorFurnitureOwners
        });
        
        await client.WriteToStreamAsync(new RoomWallItemsWriter
        {
            FurnitureOwners = wallFurnitureOwners,
            WallItems = wallItems
        });
        
        await userRepository.BroadcastDataAsync(new RoomForwardDataWriter
        {
            Room = room,
            RoomForward = false,
            EnterRoom = true,
            IsOwner = isOwner
        });
    }
}