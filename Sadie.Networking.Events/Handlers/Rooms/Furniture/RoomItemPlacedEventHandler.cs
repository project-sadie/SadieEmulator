using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Tiles;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Furniture;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

public class RoomItemPlacedEventHandler(
    SadieContext dbContext,
    RoomFurnitureItemPlacedEventParser eventParser,
    RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomFurnitureItemPlaced;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (client.Player == null || client.RoomUser == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.MissingRights);
            return;
        }
        
        var room = roomRepository.TryGetRoomById(client.Player.CurrentRoomId);
        
        if (room == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }
        
        var player = client.Player;
        var placementData = eventParser.PlacementData;
        
        if (!int.TryParse(placementData[0], out var itemId))
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        var playerItem = player.FurnitureItems.FirstOrDefault(x => x.Id == itemId);

        if (playerItem == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        if (playerItem.FurnitureItem.Type == FurnitureItemType.Floor)
        {
            await OnFloorItemAsync(
                placementData, 
                room, 
                client, 
                player, 
                playerItem,
                itemId);
        }
        else if (playerItem.FurnitureItem.Type == FurnitureItemType.Wall)
        {
            await OnWallItemAsync(placementData, room, player, playerItem, itemId, client);
        }
    }

    private async Task OnFloorItemAsync(
        IReadOnlyList<string> placementData, 
        RoomLogic room, 
        INetworkObject client, 
        PlayerLogic player, 
        PlayerFurnitureItem playerItem, 
        int itemId)
    {
        if (!int.TryParse(placementData[1], out var x) ||
            !int.TryParse(placementData[2], out var y) || 
            !int.TryParse(placementData[3], out var direction))
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        var furnitureItem = playerItem.FurnitureItem;
        
        var tiles = room.TileMap.GetTilesForSpan(
            x,
            y,
            furnitureItem.TileSpanX,
            furnitureItem.TileSpanY,
            direction);

        if (tiles.Any(t => t.State == RoomTileState.Closed))
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        var highestItemOnTiles = tiles
                .SelectMany(t => t.Items)
                .MaxBy(f => f.PositionZ);
        
        var z = (float)(highestItemOnTiles != null ? highestItemOnTiles.PositionZ + highestItemOnTiles.FurnitureItem.StackHeight : 0);
            
        var roomFurnitureItem = new RoomFurnitureItem
        {
            RoomId = room.Id,
            OwnerId = player.Id,
            OwnerUsername = player.Username,
            FurnitureItem = playerItem.FurnitureItem,
            PositionX = x,
            PositionY = y,
            PositionZ = z,
            WallPosition = string.Empty,
            Direction = (HDirection) direction,
            LimitedData = playerItem.LimitedData,
            MetaData = playerItem.MetaData,
            CreatedAt = DateTime.Now
        };

        foreach (var t in tiles)
        {
            t.Items.Add(roomFurnitureItem);
        }
        
        RoomHelpers.UpdateTileMapForTiles(tiles, room.TileMap);
        
        room.FurnitureItems.Add(roomFurnitureItem);
        player.FurnitureItems.Remove(playerItem);

        dbContext.RoomFurnitureItems.Add(roomFurnitureItem);
        await dbContext.SaveChangesAsync();

        await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter(itemId).GetAllBytes());
        
        await room.UserRepository.BroadcastDataAsync(new RoomFloorFurnitureItemPlacedWriter(
            roomFurnitureItem.Id,
            roomFurnitureItem.FurnitureItem.AssetId,
            roomFurnitureItem.PositionX,
            roomFurnitureItem.PositionY,
            roomFurnitureItem.PositionZ,
            (int) roomFurnitureItem.Direction,
            roomFurnitureItem.FurnitureItem.StackHeight,
            1,
            (int) ObjectDataKey.MapKey,
            new Dictionary<string, string>(),
            roomFurnitureItem.FurnitureItem.InteractionType,
            roomFurnitureItem.MetaData,
            roomFurnitureItem.FurnitureItem.InteractionModes,
            -1,
            roomFurnitureItem.OwnerId,
            roomFurnitureItem.OwnerUsername).GetAllBytes());
    }

    private async Task OnWallItemAsync(
        IReadOnlyList<string> placementData,
        RoomLogic room,
        Player player,
        PlayerFurnitureItem playerItem,
        int itemId,
        INetworkObject client)
    {
        var wallPosition = $"{placementData[1]} {placementData[2]} {placementData[3]}";

        var roomFurnitureItem = new RoomFurnitureItem
        {
            OwnerId = player.Id,
            OwnerUsername = player.Username,
            FurnitureItem = playerItem.FurnitureItem,
            PositionX = 0,
            PositionY = 0,
            PositionZ = 0,
            WallPosition = wallPosition,
            Direction = 0,
            LimitedData = playerItem.LimitedData,
            MetaData = playerItem.MetaData,
            CreatedAt = DateTime.Now
        };
        
        dbContext.RoomFurnitureItems.Add(roomFurnitureItem);
        await dbContext.SaveChangesAsync();

        room.FurnitureItems.Add(roomFurnitureItem);
        player.FurnitureItems.Remove(playerItem);
        
        await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter(itemId).GetAllBytes());
        await room.UserRepository.BroadcastDataAsync(new RoomWallFurnitureItemPlacedWriter(roomFurnitureItem)
            .GetAllBytes());
    }
}
    