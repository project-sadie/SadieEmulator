using AutoMapper;
using Sadie.API;
using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Bots;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerId.RoomHeightmap)]
public class RoomHeightmapEventHandler(IRoomRepository roomRepository,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService,
    IMapper mapper,
    IPlayerRepository playerRepository) : INetworkPacketEventHandler
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
        var isOwner = room.Room.OwnerId == client.Player.Id;
        
        await client.WriteToStreamAsync(new RoomRelativeMapWriter
        {
            TileMap = roomTileMap
        });
        
        await client.WriteToStreamAsync(new RoomFloorHeightMapWriter
        {
            Scale = true,
            WallHeight = -1,
            RelativeHeightmap = room.Room.Layout.Heightmap.Replace("\r\n", "\r")
        });
        
        await client.WriteToStreamAsync(new RoomWallFloorSettingsWriter
        {
            HideWalls = room.Room.Settings.HideWalls,
            WallThickness = room.Room.Settings.WallThickness,
            FloorThickness = room.Room.Settings.FloorThickness
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

        await SendFurnitureItemsAsync(room.Room, client, playerRepository);
        
        await userRepository.BroadcastDataAsync(new RoomForwardDataWriter
        {
            Room = room.Room,
            RoomForward = false,
            EnterRoom = true,
            IsOwner = isOwner,
            UsersNow = room.UserRepository.Count
        });
    }

    private async Task SendFurnitureItemsAsync(
        RoomDto room,
        INetworkObject client,
        IPlayerRepository playerRepository)
    {
        var floorItems = room.FurnitureItems
            .Where(x => x.PlayerFurnitureItem.FurnitureItem.Type == FurnitureItemType.Floor)
            .ToList();
        
        var wallItems = room.FurnitureItems
            .Where(x => x.PlayerFurnitureItem.FurnitureItem.Type == FurnitureItemType.Wall)
            .ToList();

        var tasks = floorItems
            .Select(async item => new
            {
                Key = item.PlayerFurnitureItem.PlayerId,
                Value = (await playerRepository
                        .GetPlayerByIdAsync(item.PlayerFurnitureItem.PlayerId))
                    ?.Username ?? "Unknown User"
            });

        var results = await Task.WhenAll(tasks);

        var floorFurnitureOwners = results
            .Distinct()
            .ToDictionary(x => x.Key, x => x.Value);

        var wallTasks = wallItems
            .Select(async item => new
            {
                Key = item.PlayerFurnitureItem.PlayerId,
                Value = (await playerRepository
                        .GetPlayerByIdAsync(item.PlayerFurnitureItem.PlayerId))
                    ?.Username ?? "Unknown User"
            });

        var wallResults = await Task.WhenAll(wallTasks);

        var wallFurnitureOwners = wallResults
            .Distinct()
            .ToDictionary(x => x.Key, x => x.Value);

        await client.WriteToStreamAsync(new RoomFloorItemsWriter
        {
            FloorItems = mapper.Map<List<PlayerFurnitureItemPlacementDataDto>>(floorItems),
            FurnitureOwners = floorFurnitureOwners,
            RoomFurnitureItemHelperService = roomFurnitureItemHelperService
        });
        
        await client.WriteToStreamAsync(new RoomWallItemsWriter
        {
            FurnitureOwners = wallFurnitureOwners,
            WallItems = mapper.Map<List<PlayerFurnitureItemPlacementDataDto>>(wallItems),
        });
    }
}