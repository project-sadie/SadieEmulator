using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Mapping;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Enums.Game.Rooms.Furniture;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Db.Models.Players.Furniture;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomItemPlaced)]
public class RoomItemPlacedEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomRepository roomRepository,
    IRoomFurnitureItemInteractorRepository interactorRepository,
    IRoomTileMapHelperService tileMapHelperService,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService,
    IMapper mapper,
    IPlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public required string PlacementData { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.MissingRights);
            return;
        }
        
        var room = roomRepository.TryGetRoomById(client.Player.State.CurrentRoomId);
        
        if (room == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            return;
        }
        
        var player = client.Player;
        var placementData = PlacementData.Split(" ");
        
        if (!int.TryParse(placementData[0], out var itemId))
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            return;
        }

        var playerItem = player.Player.FurnitureItems.FirstOrDefault(x => x.Id == itemId);

        if (playerItem == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            return;
        }

        if (playerItem.FurnitureItem.Type == FurnitureItemType.Floor)
        {
            if (!int.TryParse(placementData[1], out var x) ||
                !int.TryParse(placementData[2], out var y) || 
                !int.TryParse(placementData[3], out var direction))
            {
                await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
                return;
            }
            
            var pointsForPlacement = tileMapHelperService.GetPointsForPlacement(
                x, 
                y, 
                playerItem.FurnitureItem.TileSpanX,
                playerItem.FurnitureItem.TileSpanY, 
                (HDirection) direction);

            if (!pointsForPlacement.All(p => tileMapHelperService.CanPlaceAt([p], room.TileMap)))
            {
                await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            }
            
            var z = tileMapHelperService.GetItemPlacementHeight(
                room.TileMap,
                pointsForPlacement,
                room.Room.FurnitureItems);
        
            var roomFurniturePlacementData = new PlayerFurnitureItemPlacementDataDto
            {
                RoomId = room.Room.Id,
                PlayerFurnitureItemId = playerItem.Id,
                PlayerFurnitureItem = playerItem,
                PositionX = x,
                PositionY = y,
                PositionZ = z,
                WallPosition = string.Empty,
                Direction = (HDirection) direction,
                CreatedAt = DateTime.Now
            };

            playerItem.PlacementData = roomFurniturePlacementData;
            room.Room.FurnitureItems.Add(roomFurniturePlacementData);

            tileMapHelperService.UpdateTileMapsForPoints(pointsForPlacement, room.TileMap, room.Room.FurnitureItems);
        
            foreach (var user in tileMapHelperService.GetUsersAtPoints(pointsForPlacement, room.UserRepository.GetAll()))
            {
                user.CheckStatusForCurrentTile();
            }

            await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter
            {
                ItemId = playerItem.Id
            });
        
            var interactors = interactorRepository
                .GetInteractorsForType(roomFurniturePlacementData
                    .PlayerFurnitureItem
                    .FurnitureItem.InteractionType ?? "");

            foreach (var interactor in interactors)
            {
                await interactor.OnPlaceAsync(client.RoomUser.Room, roomFurniturePlacementData, client.RoomUser);
            }

            await using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var roomFurnitureItemEntity = await dbContext.PlayerFurnitureItems
                .FirstAsync(x => x.Id == roomFurniturePlacementData.PlayerFurnitureItem.Id);
            
            dbContext.Entry(roomFurnitureItemEntity).State = EntityState.Unchanged;
            
            roomFurnitureItemEntity.PlacementData = 
                mapper.Map<PlayerFurnitureItemPlacementData>(roomFurniturePlacementData);
        
            await dbContext.SaveChangesAsync();

            var roomFurnitureItem = roomFurniturePlacementData.PlayerFurnitureItem.FurnitureItem;
            
            await room.UserRepository.BroadcastDataAsync(new RoomFloorItemPlacedWriter
            {
                Id = roomFurniturePlacementData.PlayerFurnitureItemId,
                AssetId = roomFurnitureItem.AssetId,
                PositionX = roomFurniturePlacementData.PositionX,
                PositionY = roomFurniturePlacementData.PositionY,
                Direction = (int)roomFurniturePlacementData.Direction,
                PositionZ = roomFurniturePlacementData.PositionZ,
                StackHeight = 0.ToString(),
                Extra = 1,
                ObjectDataKey = (int) roomFurnitureItemHelperService.GetObjectDataKeyForItem(roomFurniturePlacementData),
                ObjectData = roomFurnitureItemHelperService.GetObjectDataForItem(roomFurniturePlacementData),
                MetaData = roomFurniturePlacementData.PlayerFurnitureItem.MetaData,
                Expires = -1,
                InteractionModes = roomFurnitureItem.InteractionModes,
                OwnerId = roomFurniturePlacementData.PlayerFurnitureItem.PlayerId,
                OwnerUsername = player.Player.Username
            });
        }
        else if (playerItem.FurnitureItem.Type == FurnitureItemType.Wall)
        {
            if (playerItem.FurnitureItem.InteractionType == FurnitureItemInteractionType.Dimmer && 
                room.Room.FurnitureItems.Any(x => x.PlayerFurnitureItem.FurnitureItem.InteractionType == FurnitureItemInteractionType.Dimmer))
            {
                await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.MaxDimmers);
                return;
            }
        
            var wallPosition = $"{placementData[1]} {placementData[2]} {placementData[3]}";

            var roomFurnitureItem = new PlayerFurnitureItemPlacementDataDto
            {
                RoomId = room.Room.Id,
                PlayerFurnitureItem = playerItem,
                PositionX = 0,
                PositionY = 0,
                PositionZ = 0,
                WallPosition = wallPosition,
                Direction = 0,
                CreatedAt = DateTime.Now
            };
        
            room.Room.FurnitureItems.Add(roomFurnitureItem);
        
            await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter
            {
                ItemId = itemId
            });
        
            var interactors = interactorRepository
                .GetInteractorsForType(roomFurnitureItem
                    .PlayerFurnitureItem
                    .FurnitureItem.InteractionType ?? "");
        
            foreach (var interactor in interactors)
            {
                await interactor.OnPlaceAsync(client.RoomUser.Room, roomFurnitureItem, client.RoomUser);
            }
        
            var owner = await playerRepository.GetPlayerByIdAsync(
                roomFurnitureItem.PlayerFurnitureItem.PlayerId);
        
            await room.UserRepository.BroadcastDataAsync(new RoomWallFurnitureItemPlacedWriter
            {
                RoomFurnitureItem = roomFurnitureItem,
                OwnerUsername = owner?.Username ?? "Unknown User"
            });

            await using var dbContext = await dbContextFactory.CreateDbContextAsync();
            dbContext.Entry(roomFurnitureItem).State = EntityState.Added;
            await dbContext.SaveChangesAsync();
        }
    }
}
    