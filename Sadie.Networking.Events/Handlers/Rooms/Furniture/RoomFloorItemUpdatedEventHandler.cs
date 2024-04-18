using Sadie.Database;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Tiles;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Furniture;
using Sadie.Networking.Packets;
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

        var tilesOn = room.TileMap.GetTilesForSpan(
            roomFurnitureItem.PositionX,
            roomFurnitureItem.PositionY,
            roomFurnitureItem.FurnitureItem.TileSpanX,
            roomFurnitureItem.FurnitureItem.TileSpanY,
            (int) roomFurnitureItem.Direction);

        foreach (var t in tilesOn)
        {
            t?.Items.Remove(roomFurnitureItem);
        }

        var position = new HPoint(
            eventParser.X,
            eventParser.Y, 
            roomFurnitureItem.PositionZ);

        var direction = eventParser.Direction;
        
        var newTiles = room.TileMap.GetTilesForSpan(
            position.X,
            position.Y,
            roomFurnitureItem.FurnitureItem.TileSpanX,
            roomFurnitureItem.FurnitureItem.TileSpanY,
            (int)direction);

        if (tilesOn.Any(x => x.State == RoomTileState.Closed))
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }
        
        RoomHelpers.UpdateTileMapForTiles(tilesOn, room.TileMap);

        foreach (var t in newTiles)
        {
            t.Items.Add(roomFurnitureItem);
        }
        
        RoomHelpers.UpdateTileMapForTiles(newTiles, room.TileMap);

        var movedTiles = roomFurnitureItem.PositionX != position.X || roomFurnitureItem.PositionY != position.Y;

        roomFurnitureItem.PositionX = position.X;
        roomFurnitureItem.PositionY = position.Y;
        roomFurnitureItem.PositionZ = position.Z;
        roomFurnitureItem.Direction = direction;
        
        if (movedTiles)
        {
            foreach (var userOnTile in tilesOn.SelectMany(x => x.Users.Values))
            {
                userOnTile.CheckStatusForCurrentTile();
            }
        }

        foreach (var userOnTile in newTiles.SelectMany(x => x.Users.Values))
        {
            userOnTile.CheckStatusForCurrentTile();
        }
        
        await dbContext.SaveChangesAsync();
        await BroadcastUpdateAsync(room, roomFurnitureItem);
    }

    private static async Task BroadcastUpdateAsync(RoomLogic room, RoomFurnitureItem roomFurnitureItem)
    {
        await room.UserRepository.BroadcastDataAsync(new RoomFloorItemUpdatedWriter(
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