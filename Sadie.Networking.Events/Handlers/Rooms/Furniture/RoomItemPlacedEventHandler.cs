using System.Drawing;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Mapping;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomFurnitureItemPlaced)]
public class RoomItemPlacedEventHandler(
    SadieContext dbContext,
    RoomFurnitureItemPlacedEventParser eventParser,
    RoomRepository roomRepository) : INetworkPacketEventHandler
{
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

        if (!RoomTileMapHelpers
            .GetPointsForPlacement(x, y, playerItem.FurnitureItem.TileSpanX, playerItem.FurnitureItem.TileSpanY,
                direction).All(x => RoomTileMapHelpers.CanPlaceAt((List<Point>)[new Point(x.X, x.Y)], room.TileMap)))
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        var points = RoomTileMapHelpers.GetPointsForPlacement(x, y, playerItem.FurnitureItem.TileSpanX,
            playerItem.FurnitureItem.TileSpanY, direction);

        var z = 0; // TODO: Calculate this
        
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

        player.FurnitureItems.Remove(playerItem);
        dbContext.PlayerFurnitureItems.Remove(playerItem);
        
        room.FurnitureItems.Add(roomFurnitureItem);
        dbContext.RoomFurnitureItems.Add(roomFurnitureItem);

        RoomTileMapHelpers.UpdateTileStatesForPoints(points, room.TileMap, room.FurnitureItems);
        
        await dbContext.SaveChangesAsync();

        await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter
        {
            ItemId = itemId
        });

        await room.UserRepository.BroadcastDataAsync(new RoomFloorItemPlacedWriter
        {
            Id = roomFurnitureItem.Id,
            AssetId = roomFurnitureItem.FurnitureItem.AssetId,
            PositionX = roomFurnitureItem.PositionX,
            PositionY = roomFurnitureItem.PositionY,
            Direction = (int)roomFurnitureItem.Direction,
            PositionZ = roomFurnitureItem.PositionZ,
            StackHeight = 0,
            Extra = 1,
            ObjectDataKey = (int)ObjectDataKey.MapKey,
            ObjectData = new Dictionary<string, string>(),
            MetaData = roomFurnitureItem.MetaData,
            Expires = -1,
            InteractionModes = 0,
            OwnerId = roomFurnitureItem.OwnerId,
            OwnerUsername = roomFurnitureItem.OwnerUsername
        });
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
        
        await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter
        {
            ItemId = itemId
        });
        
        await room.UserRepository.BroadcastDataAsync(new RoomWallFurnitureItemPlacedWriter
        {
            RoomFurnitureItem = roomFurnitureItem
        });
    }
}
    