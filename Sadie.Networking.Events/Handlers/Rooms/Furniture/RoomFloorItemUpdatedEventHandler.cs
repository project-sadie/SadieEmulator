using Sadie.Database;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Tiles;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Furniture;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

public class RoomFloorItemUpdatedEventHandler(
    SadieContext dbContext,
    RoomFloorItemUpdatedEventParser eventParser,
    RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomFloorFurnitureItemUpdated;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }

        var itemId = eventParser.ItemId;
        
        var room = roomRepository.TryGetRoomById(client.Player.CurrentRoomId);
        
        if (room == null)
        {
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.MissingRights);
            return;
        }

        var roomFurnitureItem = room.FurnitureItems.FirstOrDefault(x => x.Id == itemId);

        if (roomFurnitureItem == null)
        {
            return;
        }
        
        var currentTile = room.TileMap.FindTile(
            roomFurnitureItem.PositionX, roomFurnitureItem.PositionY);

        currentTile?.Items.Remove(roomFurnitureItem);

        var position = new HPoint(
            eventParser.X,
            eventParser.Y, 
            roomFurnitureItem.PositionZ);

        var direction = eventParser.Direction;
        var tile = room.TileMap.FindTile(position.X, position.Y);

        if (tile == null || tile.State == RoomTileState.Closed)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }
        
        if (currentTile != null)
        {
            RoomHelpers.UpdateTileMapForTile(currentTile, room.TileMap);
        }

        tile.Items.Add(roomFurnitureItem);
        RoomHelpers.UpdateTileMapForTile(tile, room.TileMap);

        var movedTiles = roomFurnitureItem.PositionX != position.X || roomFurnitureItem.PositionY != position.Y;

        roomFurnitureItem.PositionX = position.X;
        roomFurnitureItem.PositionY = position.Y;
        roomFurnitureItem.PositionZ = position.Z;
        roomFurnitureItem.Direction = direction;
        
        if (movedTiles)
        {
            foreach (var userOnTile in tile.Users.Values)
            {
                userOnTile.CheckStatusForCurrentTile();
            }
        }

        if (currentTile != null)
        {
            foreach (var userOnTile in currentTile.Users.Values)
            {
                userOnTile.CheckStatusForCurrentTile();
            }
        }
        
        await dbContext.SaveChangesAsync();
        await BroadcastUpdateAsync(room, roomFurnitureItem);
    }

    private static async Task BroadcastUpdateAsync(RoomLogic room, RoomFurnitureItem roomFurnitureItem)
    {
        await room.UserRepository.BroadcastDataAsync(new RoomFloorFurnitureItemUpdatedWriter(
            roomFurnitureItem.Id,
            roomFurnitureItem.FurnitureItem.AssetId,
            roomFurnitureItem.PositionX,
            roomFurnitureItem.PositionY,
            roomFurnitureItem.PositionZ,
            (int) roomFurnitureItem.Direction,
            0,
            1,
            (int) ObjectDataKey.MapKey,
            new Dictionary<string, string>(),
            roomFurnitureItem.FurnitureItem.InteractionType,
            roomFurnitureItem.MetaData,
            roomFurnitureItem.FurnitureItem.InteractionModes,
            -1,
            roomFurnitureItem.OwnerId).GetAllBytes());
    }
}