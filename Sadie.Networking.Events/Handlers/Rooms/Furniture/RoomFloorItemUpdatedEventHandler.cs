using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Mapping;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Rooms.Furniture;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomFloorFurnitureItemUpdated)]
public class RoomFloorItemUpdatedEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
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

        var furnitureItem = roomFurnitureItem
            .PlayerFurnitureItem
            .FurnitureItem;
        
        var oldPoints = tileMapHelperService.GetPointsForPlacement(
            roomFurnitureItem.PositionX, 
            roomFurnitureItem.PositionY, 
            furnitureItem.TileSpanX,
            furnitureItem.TileSpanY, 
            roomFurnitureItem.Direction);

        var newPoints = tileMapHelperService.GetPointsForPlacement(
            X, Y, 
            furnitureItem.TileSpanX,
            furnitureItem.TileSpanY, 
            (HDirection) Direction);

        tileMapHelperService.UpdateTileMapsForPoints(oldPoints, 
            room.TileMap,
            room
                .FurnitureItems
                .Except([roomFurnitureItem])
                .ToList());
        
        var rotatingSingleTileItem = newPoints.Count == 1 && 
             newPoints[0].X == roomFurnitureItem.PositionX &&
             newPoints[0].Y == roomFurnitureItem.PositionY;

        var checkPointsForUsers = furnitureItem is
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

        roomFurnitureItem.PositionX = X;
        roomFurnitureItem.PositionY = Y;
        roomFurnitureItem.PositionZ = z;
        roomFurnitureItem.Direction = (HDirection) Direction;
        
        room.TileMap.Map[roomFurnitureItem.PositionY, roomFurnitureItem.PositionX] =
            (short) tileMapHelperService.GetTileState(
                roomFurnitureItem.PositionX, 
                roomFurnitureItem.PositionY,
                room.FurnitureItems);
        
        var interactors = interactorRepository
            .GetInteractorsForType(furnitureItem.InteractionType ?? "");

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