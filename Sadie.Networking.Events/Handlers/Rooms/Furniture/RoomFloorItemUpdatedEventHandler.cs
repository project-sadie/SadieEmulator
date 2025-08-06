using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Db;
using Sadie.Enums.Game.Rooms.Furniture;
using Sadie.Enums.Unsorted;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomFloorFurnitureItemUpdated)]
public class RoomFloorItemUpdatedEventHandler(
    IDbContextFactory<SadieContext> dbContextFactory,
    IRoomRepository roomRepository,
    IRoomFurnitureItemInteractorRepository interactorRepository,
    IRoomTileMapHelperService tileMapHelperService,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : INetworkPacketEventHandler
{
    public int ItemId { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
    public int Direction { get; init; }
    
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
        
        var oldPoints = tileMapHelperService.GetPointsForPlacement(
            roomFurnitureItem.PositionX, 
            roomFurnitureItem.PositionY, 
            roomFurnitureItem.FurnitureItem.TileSpanX,
            roomFurnitureItem.FurnitureItem.TileSpanY, 
            (int) roomFurnitureItem.Direction);

        var newPoints = tileMapHelperService.GetPointsForPlacement(
            X, Y, 
            roomFurnitureItem.FurnitureItem.TileSpanX,
            roomFurnitureItem.FurnitureItem.TileSpanY, 
            Direction);

        tileMapHelperService.UpdateTileMapsForPoints(oldPoints, 
            room.TileMap,
            room
                .FurnitureItems
                .Except([roomFurnitureItem])
                .ToList());
        
        var rotatingSingleTileItem = newPoints.Count == 1 && 
             newPoints[0].X == roomFurnitureItem.PositionX &&
             newPoints[0].Y == roomFurnitureItem.PositionY;

        var checkPointsForUsers = roomFurnitureItem.FurnitureItem is
        {
            CanSit: false, 
            CanLay: false
        };
        
        if (!rotatingSingleTileItem && 
            !tileMapHelperService.CanPlaceAt(newPoints, room.TileMap, checkPointsForUsers))
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            return;
        }

        var z = tileMapHelperService.GetItemPlacementHeight(
            room.TileMap, 
            newPoints, 
            room
                .FurnitureItems
                .Except([roomFurnitureItem])
                .ToList());
        
        var direction = Direction;

        roomFurnitureItem.PositionX = X;
        roomFurnitureItem.PositionY = Y;
        roomFurnitureItem.PositionZ = z;
        roomFurnitureItem.Direction = (HDirection) direction;
        
        room.TileMap.Map[roomFurnitureItem.PositionY, roomFurnitureItem.PositionX] =
            (short) tileMapHelperService.GetTileState(
                roomFurnitureItem.PositionX, 
                roomFurnitureItem.PositionY,
                room.FurnitureItems);
        
        var interactors = interactorRepository
            .GetInteractorsForType(roomFurnitureItem.FurnitureItem.InteractionType);

        foreach (var interactor in interactors)
        {
            await interactor.OnMoveAsync(room, roomFurnitureItem, client.RoomUser);
        }
        
        foreach (var user in tileMapHelperService.GetUsersAtPoints(oldPoints, room.UserRepository.GetAll()))
        {
            user.CheckStatusForCurrentTile();
        }
        
        foreach (var user in tileMapHelperService.GetUsersAtPoints(newPoints, room.UserRepository.GetAll()))
        {
            user.CheckStatusForCurrentTile();
        }
        
        tileMapHelperService.UpdateTileMapsForPoints(oldPoints, room.TileMap, room.FurnitureItems);            
        tileMapHelperService.UpdateTileMapsForPoints(newPoints, room.TileMap, room.FurnitureItems);

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(roomFurnitureItem).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
        
        await roomFurnitureItemHelperService.BroadcastItemUpdateToRoomAsync(room, roomFurnitureItem);
    }
}