using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Game.Rooms.Furniture;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomItemPlaced)]
public class RoomItemPlacedEventHandler(
    SadieContext dbContext,
    IRoomRepository roomRepository,
    IRoomFurnitureItemInteractorRepository interactorRepository,
    IRoomTileMapHelperService tileMapHelperService,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : INetworkPacketEventHandler
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

        var playerItem = player.FurnitureItems.FirstOrDefault(x => x.Id == itemId);

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
            
            var pointsForPlacement = tileMapHelperService.GetPointsForPlacement(x, y, playerItem.FurnitureItem.TileSpanX,
                playerItem.FurnitureItem.TileSpanY, direction);

            if (!pointsForPlacement.All(p => tileMapHelperService.CanPlaceAt([p], room.TileMap)))
            {
                await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            }
            
            var z = tileMapHelperService.GetItemPlacementHeight(
                room.TileMap,
                pointsForPlacement,
                room.FurnitureItems);
        
            var roomFurnitureItem = new PlayerFurnitureItemPlacementData
            {
                RoomId = room.Id,
                PlayerFurnitureItemId = playerItem.Id,
                PlayerFurnitureItem = playerItem,
                PositionX = x,
                PositionY = y,
                PositionZ = z,
                WallPosition = string.Empty,
                Direction = (HDirection) direction,
                CreatedAt = DateTime.Now
            };

            playerItem.PlacementData = roomFurnitureItem;
            room.FurnitureItems.Add(roomFurnitureItem);

            tileMapHelperService.UpdateTileMapsForPoints(pointsForPlacement, room.TileMap, room.FurnitureItems);
        
            foreach (var user in tileMapHelperService.GetUsersAtPoints(pointsForPlacement, room.UserRepository.GetAll()))
            {
                user.CheckStatusForCurrentTile();
            }

            await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter
            {
                ItemId = playerItem.Id
            });

            dbContext.Entry(roomFurnitureItem.PlayerFurnitureItem).State = EntityState.Unchanged;
            dbContext.RoomFurnitureItems.Add(roomFurnitureItem);
            
            await dbContext.SaveChangesAsync();
            
            var interactors = interactorRepository
                .GetInteractorsForType(roomFurnitureItem.FurnitureItem.InteractionType);

            foreach (var interactor in interactors)
            {
                await interactor.OnPlaceAsync(client.RoomUser.Room, roomFurnitureItem, client.RoomUser);
            }

            await room.UserRepository.BroadcastDataAsync(new RoomFloorItemPlacedWriter
            {
                Id = roomFurnitureItem.PlayerFurnitureItemId,
                AssetId = roomFurnitureItem.FurnitureItem.AssetId,
                PositionX = roomFurnitureItem.PositionX,
                PositionY = roomFurnitureItem.PositionY,
                Direction = (int)roomFurnitureItem.Direction,
                PositionZ = roomFurnitureItem.PositionZ,
                StackHeight = 0.ToString(),
                Extra = 1,
                ObjectDataKey = (int) roomFurnitureItemHelperService.GetObjectDataKeyForItem(roomFurnitureItem),
                ObjectData = roomFurnitureItemHelperService.GetObjectDataForItem(roomFurnitureItem),
                MetaData = roomFurnitureItem.PlayerFurnitureItem.MetaData,
                Expires = -1,
                InteractionModes = roomFurnitureItem.FurnitureItem.InteractionModes,
                OwnerId = roomFurnitureItem.PlayerFurnitureItem.PlayerId,
                OwnerUsername = roomFurnitureItem.PlayerFurnitureItem.Player.Username
            });
        }
        else if (playerItem.FurnitureItem.Type == FurnitureItemType.Wall)
        {
            if (playerItem.FurnitureItem.InteractionType == FurnitureItemInteractionType.Dimmer && 
                room.FurnitureItems.Any(x => x.FurnitureItem.InteractionType == FurnitureItemInteractionType.Dimmer))
            {
                await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.MaxDimmers);
                return;
            }
        
            var wallPosition = $"{placementData[1]} {placementData[2]} {placementData[3]}";

            var roomFurnitureItem = new PlayerFurnitureItemPlacementData
            {
                RoomId = room.Id,
                PlayerFurnitureItem = playerItem,
                PositionX = 0,
                PositionY = 0,
                PositionZ = 0,
                WallPosition = wallPosition,
                Direction = 0,
                CreatedAt = DateTime.Now
            };
        
            room.FurnitureItems.Add(roomFurnitureItem);
        
            await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter
            {
                ItemId = itemId
            });

            dbContext.Entry(roomFurnitureItem).State = EntityState.Added;
            await dbContext.SaveChangesAsync();
        
            var interactors = interactorRepository
                .GetInteractorsForType(roomFurnitureItem.FurnitureItem.InteractionType);
        
            foreach (var interactor in interactors)
            {
                await interactor.OnPlaceAsync(client.RoomUser.Room, roomFurnitureItem, client.RoomUser);
            }
        
            await room.UserRepository.BroadcastDataAsync(new RoomWallFurnitureItemPlacedWriter
            {
                RoomFurnitureItem = roomFurnitureItem
            });
        }
    }
}
    