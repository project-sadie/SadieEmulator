using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomFloorFurnitureItemUpdated)]
public class RoomFloorItemUpdatedEventHandler(
    SadieContext dbContext,
    RoomRepository roomRepository,
    RoomFurnitureItemInteractorRepository interactorRepository) : INetworkPacketEventHandler
{
    public int ItemId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Direction { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            return;
        }

        var itemId = ItemId;
        
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

        var roomFurnitureItem = room.FurnitureItems.FirstOrDefault(x => x.PlayerFurnitureItemId == itemId);

        if (roomFurnitureItem == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }
        
        var oldPoints = RoomTileMapHelpers.GetPointsForPlacement(
            roomFurnitureItem.PositionX, 
            roomFurnitureItem.PositionY, 
            roomFurnitureItem.FurnitureItem.TileSpanX,
            roomFurnitureItem.FurnitureItem.TileSpanY, 
            (int) roomFurnitureItem.Direction);

        var newPoints = RoomTileMapHelpers.GetPointsForPlacement(
            X, Y, 
            roomFurnitureItem.FurnitureItem.TileSpanX,
            roomFurnitureItem.FurnitureItem.TileSpanY, 
            Direction);

        RoomTileMapHelpers.UpdatePointListForRoomMap(oldPoints, 
            room.TileMap,
            room
                .FurnitureItems
                .Except([roomFurnitureItem])
                .ToList());
        
        var rotatingSingleTileItem = newPoints.Count == 1 && 
             newPoints[0].X == roomFurnitureItem.PositionX &&
             newPoints[0].Y == roomFurnitureItem.PositionY;
        
        if (!rotatingSingleTileItem && 
            !RoomTileMapHelpers.CanPlaceAt(newPoints, room.TileMap))
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.CantSetItem);
            return;
        }

        var z = RoomTileMapHelpers.GetItemPlacementHeight(
            room.TileMap, 
            newPoints, 
            room
                .FurnitureItems
                .Except([roomFurnitureItem])
                .ToList());

        var position = new HPoint(
            X,
            Y,
            z);

        var direction = Direction;

        roomFurnitureItem.PositionX = position.X;
        roomFurnitureItem.PositionY = position.Y;
        roomFurnitureItem.PositionZ = position.Z;
        roomFurnitureItem.Direction = (HDirection) direction;
        
        room.TileMap.Map[roomFurnitureItem.PositionY, roomFurnitureItem.PositionX] =
            RoomTileMapHelpers.GetStateNumberForTile(
                roomFurnitureItem.PositionX, 
                roomFurnitureItem.PositionY,
                room.FurnitureItems);
        
        var interactor = interactorRepository.GetInteractorForType(roomFurnitureItem.FurnitureItem.InteractionType);

        if (interactor != null)
        {
            await interactor.OnMoveAsync(room, roomFurnitureItem, client.RoomUser);
        }
        
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

        dbContext.Entry(roomFurnitureItem).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
        
        await BroadcastUpdateAsync(room, roomFurnitureItem);
    }

    private static async Task BroadcastUpdateAsync(IRoomLogic room, PlayerFurnitureItemPlacementData roomFurnitureItem)
    {
        await room.UserRepository.BroadcastDataAsync(new RoomFloorItemUpdatedWriter
        {
            Id = roomFurnitureItem.PlayerFurnitureItemId,
            AssetId = roomFurnitureItem.FurnitureItem.AssetId,
            PositionX = roomFurnitureItem.PositionX,
            PositionY = roomFurnitureItem.PositionY,
            Direction = (int)roomFurnitureItem.Direction,
            PositionZ = roomFurnitureItem.PositionZ,
            StackHeight = 0.ToString(),
            Extra = 0,
            ObjectDataKey = 0,
            ExtraData = roomFurnitureItem.PlayerFurnitureItem.MetaData,
            Expires = -1,
            InteractionModes = 0,
            OwnerId = roomFurnitureItem.PlayerFurnitureItem.PlayerId
        });
    }
}