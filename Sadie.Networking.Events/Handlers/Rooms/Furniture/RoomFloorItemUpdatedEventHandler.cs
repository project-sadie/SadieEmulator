using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Enums.Game.Rooms.Furniture;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomFloorFurnitureItemUpdated)]
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
        
        var room = roomRepository.TryGetRoomById(client.Player.State.CurrentRoomId);
        
        if (room == null)
        {
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.MissingRights);
            return;
        }

        var roomFurnitureItem = room.FurnitureItems.FirstOrDefault(x => x.PlayerFurnitureItemId == itemId);

        if (roomFurnitureItem == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
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

        RoomTileMapHelpers.UpdateTileMapsForPoints(oldPoints, 
            room.TileMap,
            room
                .FurnitureItems
                .Except([roomFurnitureItem])
                .ToList());
        
        var rotatingSingleTileItem = newPoints.Count == 1 && 
             newPoints[0].X == roomFurnitureItem.PositionX &&
             newPoints[0].Y == roomFurnitureItem.PositionY;
        
        if (!rotatingSingleTileItem && 
            !RoomTileMapHelpers.CanPlace(newPoints, room.TileMap))
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
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
            (short) RoomTileMapHelpers.GetTileState(
                roomFurnitureItem.PositionX, 
                roomFurnitureItem.PositionY,
                room.FurnitureItems);
        
        var interactor = interactorRepository.GetInteractorForType(roomFurnitureItem.FurnitureItem.InteractionType);

        if (interactor != null)
        {
            await interactor.OnMoveAsync(room, roomFurnitureItem, client.RoomUser);
        }
        
        foreach (var user in RoomTileMapHelpers.GetUsersAtPoints(oldPoints, room.UserRepository.GetAll()))
        {
            user.CheckStatusForCurrentTile();
        }
        
        foreach (var user in RoomTileMapHelpers.GetUsersAtPoints(newPoints, room.UserRepository.GetAll()))
        {
            user.CheckStatusForCurrentTile();
        }
        
        RoomTileMapHelpers.UpdateTileMapsForPoints(oldPoints, room.TileMap, room.FurnitureItems);            
        RoomTileMapHelpers.UpdateTileMapsForPoints(newPoints, room.TileMap, room.FurnitureItems);

        dbContext.Entry(roomFurnitureItem).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
        
        await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, roomFurnitureItem);
    }
}