using Sadie.Database;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomFloorFurnitureItemUpdated)]
public class RoomFloorItemUpdatedEventHandler(
    SadieContext dbContext,
    RoomFloorItemUpdatedEventParser eventParser,
    RoomRepository roomRepository) : INetworkPacketEventHandler
{
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

        var newPoints = RoomTileMapHelpers.GetPointsForPlacement(
            eventParser.X, eventParser.Y, 
            roomFurnitureItem.FurnitureItem.TileSpanX,
            roomFurnitureItem.FurnitureItem.TileSpanY, 
            eventParser.Direction);
        
        if (!RoomTileMapHelpers.CanPlaceAt(newPoints, room.TileMap))
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        var position = new HPoint(
            eventParser.X,
            eventParser.Y, 
            roomFurnitureItem.PositionZ);

        var direction = eventParser.Direction;
        
        var oldPoints = RoomTileMapHelpers.GetPointsForPlacement(
            roomFurnitureItem.PositionX, 
            roomFurnitureItem.PositionY, 
            roomFurnitureItem.FurnitureItem.TileSpanX,
            roomFurnitureItem.FurnitureItem.TileSpanY, 
            (int) roomFurnitureItem.Direction);

        roomFurnitureItem.PositionX = position.X;
        roomFurnitureItem.PositionY = position.Y;
        roomFurnitureItem.PositionZ = position.Z;
        roomFurnitureItem.Direction = (HDirection) direction;
        
        foreach (var user in RoomTileMapHelpers.GetUsersForPoints(oldPoints, room.UserRepository.GetAll()))
        {
            user.CheckStatusForCurrentTile();
        }
        
        foreach (var user in RoomTileMapHelpers.GetUsersForPoints(newPoints, room.UserRepository.GetAll()))
        {
            user.CheckStatusForCurrentTile();
        }
        
        RoomTileMapHelpers.UpdateTileStatesForPoints(oldPoints, room.TileMap, room.FurnitureItems);            
        RoomTileMapHelpers.UpdateTileStatesForPoints(newPoints, room.TileMap, room.FurnitureItems);
        
        await dbContext.SaveChangesAsync();
        await BroadcastUpdateAsync(room, roomFurnitureItem);
    }

    private static async Task BroadcastUpdateAsync(RoomLogic room, RoomFurnitureItem roomFurnitureItem)
    {
        await room.UserRepository.BroadcastDataAsync(new RoomFloorItemUpdatedWriter
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
            OwnerId = roomFurnitureItem.OwnerId
        });
    }
}