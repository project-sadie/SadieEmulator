using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Database.Models.Catalog;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Enums.Game.Rooms;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Bots;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Players.Purse;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Networking.Events;

public static class RoomHelpersDirty
{
    public static async Task<bool> TryChargeForCatalogItemPurchaseAsync(INetworkClient client, CatalogItem item, int amount)
    {
        var costInCredits = item.CostCredits * amount;
        var costInPoints = item.CostPoints * amount;

        var playerData = client.Player.Data;
        
        if (playerData.CreditBalance < costInCredits || 
            (item.CostPointsType == 0 && playerData.PixelBalance < costInPoints) ||
            (item.CostPointsType != 0 && playerData.SeasonalBalance < costInPoints))
        {
            return false;
        }

        if (costInCredits > 0)
        {
            playerData.CreditBalance -= costInCredits;
        
            await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter
            {
                Credits = playerData.CreditBalance
            });
        }

        if (costInPoints > 0)
        {
            if (item.CostPointsType == 0)
            {
                playerData.PixelBalance -= costInPoints;
            }
            else
            {
                playerData.SeasonalBalance -= costInPoints;
            }
        
            await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter
            {
                PixelBalance = playerData.PixelBalance,
                SeasonalBalance = playerData.SeasonalBalance,
                GotwPoints = playerData.GotwPoints
            });
        }

        return true;
    }

    private static RoomControllerLevel GetControllerLevelForUser(Room room, int userId)
    {
        var controllerLevel = RoomControllerLevel.None;
        
        if (room.PlayerRights.FirstOrDefault(x => x.PlayerId == userId) != null)
        {
            controllerLevel = RoomControllerLevel.Rights;
        }

        if (room.OwnerId == userId)
        {
            controllerLevel = RoomControllerLevel.Owner;
        }

        return controllerLevel;
    }

    private static void CreateRoomVisitForPlayer(PlayerLogic player, int roomId)
    {
        player.State.RoomVisits.Add(new PlayerRoomVisit
        {
            PlayerId = player.Id,
            RoomId = roomId,
            CreatedAt = DateTime.Now
        });
    }

    public static RoomBot CreateBot(
        int id, 
        Room room, 
        Point point,
        RoomBotFactory roomBotFactory)
    {
        return roomBotFactory.Create(room, id, point);
    }

    private static RoomUser CreateUserForEntry(
        RoomUserFactory roomUserFactory, 
        RoomLogic room, 
        PlayerLogic player,
        Point spawnPoint,
        HDirection direction)
    {
        var z = 0; // TODO: Calculate this
        
        return roomUserFactory.Create(
            room,
            player.NetworkObject,
            player.Id,
            spawnPoint,
            z,
            direction,
            direction,
            player,
            GetControllerLevelForUser(room, player.Id));
    }
    
    public static async Task EnterRoomAsync<T>(
        INetworkClient client, 
        RoomLogic room, 
        ILogger<T> logger, 
        RoomUserFactory roomUserFactory)
    {
        var player = client.Player;
        var entryPoint = new Point(room.Layout.DoorX, room.Layout.DoorY);
        var entryDirection = room.Layout.DoorDirection;
        var teleport = player.State.Teleport;

        if (teleport != null)
        {
            entryPoint = new Point(teleport.PositionX, teleport.PositionY);
            entryDirection = teleport.Direction;
            
            player.State.Teleport = null;
        }
        
        var roomUser = CreateUserForEntry(roomUserFactory, room, player, entryPoint, entryDirection);
        
        if (!room.UserRepository.TryAdd(roomUser))
        {
            logger.LogError($"Failed to add user {player.Id} to room {room.Id}");
            return;
        }
        
        if (teleport != null)
        {
            var squareInFront = RoomTileMapHelpers
                .GetPointInFront(teleport.PositionX, teleport.PositionY, teleport.Direction);

            roomUser.WalkToPoint(squareInFront);
            
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(800);
                
                teleport.MetaData = "0";
                await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, teleport);
            });
        }
        
        CreateRoomVisitForPlayer(player, room.Id);
        player.CurrentRoomId = room.Id;

        room.TileMap.AddUserToMap(entryPoint, roomUser);
        roomUser.ApplyFlatCtrlStatus();
        
        client.RoomUser = roomUser;
        await SendRoomEntryPacketsToUserAsync(client, room);
    }

    private static async Task SendRoomEntryPacketsToUserAsync(INetworkClient client, Room room)
    {
        var player = client.Player;
        var roomUser = client.RoomUser;
        var canLikeRoom = player.RoomLikes.FirstOrDefault(x => x.RoomId == room.Id) == null;
        
        await client.WriteToStreamAsync(new RoomDataWriter
        {
            LayoutName = room.Layout.Name,
            RoomId = room.Id
        });

        if (room.PaintSettings.FloorPaint != "0.0")
        {
            await client.WriteToStreamAsync(new RoomPaintWriter
            {
                Type = "floor",
                Value = room.PaintSettings.FloorPaint
            });
        }

        if (room.PaintSettings.WallPaint != "0.0")
        {
            await client.WriteToStreamAsync(new RoomPaintWriter
            {
                Type = "wallpaper",
                Value = room.PaintSettings.WallPaint
            });
        }
        
        await client.WriteToStreamAsync(new RoomPaintWriter
        {
            Type = "landscape",
            Value = room.PaintSettings.LandscapePaint
        });
        
        await client.WriteToStreamAsync(new RoomScoreWriter
        {
            Score = room.PlayerLikes.Count,
            CanUpvote = canLikeRoom
        });
        
        await client.WriteToStreamAsync(new RoomPromotionWriter());

        var owner = room.OwnerId == player.Id;
        
        await client.WriteToStreamAsync(new RoomWallFloorSettingsWriter
        {
            HideWalls = room.Settings.HideWalls,
            WallThickness = room.Settings.WallThickness,
            FloorThickness = room.Settings.FloorThickness
        });
        
        await client.WriteToStreamAsync(new RoomPaneWriter
        {
            RoomId = room.Id,
            Owner = owner
        });
        
        await client.WriteToStreamAsync(new RoomRightsWriter
        {
            ControllerLevel = (int)roomUser.ControllerLevel
        });
        
        if (owner)
        {
            await client.WriteToStreamAsync(new RoomOwnerWriter());
        }

        await client.WriteToStreamAsync(new RoomLoadedWriter());
    }

    public static async Task<bool> TryChargeForClubOfferPurchaseAsync(INetworkClient client, CatalogClubOffer offer)
    {
        var costInCredits = offer.CostCredits;
        var costInPoints = offer.CostPoints;

        var playerData = client.Player.Data;
        
        if (playerData.CreditBalance < costInCredits || 
            (offer.CostPointsType == 0 && playerData.PixelBalance < costInPoints) ||
            (offer.CostPointsType != 0 && playerData.SeasonalBalance < costInPoints))
        {
            return false;
        }

        if (costInCredits > 0)
        {
            playerData.CreditBalance -= costInCredits;
        
            await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter
            {
                Credits = playerData.CreditBalance
            });
        }

        if (costInPoints > 0)
        {
            if (offer.CostPointsType == 0)
            {
                playerData.PixelBalance -= costInPoints;
            }
            else
            {
                playerData.SeasonalBalance -= costInPoints;
            }
        }
        
        await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter
        {
            PixelBalance = playerData.PixelBalance,
            SeasonalBalance = playerData.SeasonalBalance,
            GotwPoints = playerData.GotwPoints
        });

        return true;
    }
    
    public static async Task OnPlaceFloorItemAsync(
        IReadOnlyList<string> placementData, 
        RoomLogic room, 
        INetworkClient client, 
        PlayerLogic player, 
        PlayerFurnitureItem playerItem, 
        int itemId,
        SadieContext dbContext,
        RoomFurnitureItemInteractorRepository interactorRepository)
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
        room.FurnitureItems.Add(roomFurnitureItem);

        RoomTileMapHelpers.UpdateTileStatesForPoints(points, room.TileMap, room.FurnitureItems);

        await client.WriteToStreamAsync(new PlayerInventoryRemoveItemWriter
        {
            ItemId = itemId
        });
        
        dbContext.Entry(playerItem).State = EntityState.Deleted;
        dbContext.Entry(roomFurnitureItem).State = EntityState.Added;
        
        await dbContext.SaveChangesAsync();

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
            ObjectDataKey = (int)ObjectDataKey.LegacyKey,
            MetaData = roomFurnitureItem.MetaData,
            Expires = -1,
            InteractionModes = roomFurnitureItem.FurnitureItem.InteractionModes,
            OwnerId = roomFurnitureItem.OwnerId,
            OwnerUsername = roomFurnitureItem.OwnerUsername
        });
        
        var interactor = interactorRepository.GetInteractorForType(roomFurnitureItem.FurnitureItem.InteractionType);

        if (interactor != null)
        {
            await interactor.OnPlaceAsync(room, roomFurnitureItem, client.RoomUser);
        }
    }

    public static async Task OnPlaceWallItemAsync(
        IReadOnlyList<string> placementData,
        RoomLogic room,
        Player player,
        PlayerFurnitureItem playerItem,
        int itemId,
        INetworkClient client,
        SadieContext dbContext,
        RoomFurnitureItemInteractorRepository interactorRepository)
    {
        if (playerItem.FurnitureItem.InteractionType == "dimmer" && 
            room.FurnitureItems.Any(x => x.FurnitureItem.InteractionType == "dimmer"))
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, FurniturePlacementError.MaxDimmers);
            return;
        }
        
        var wallPosition = $"{placementData[1]} {placementData[2]} {placementData[3]}";

        var roomFurnitureItem = new RoomFurnitureItem
        {
            RoomId = room.Id,
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

        if (roomFurnitureItem.FurnitureItem.InteractionType == "dimmer" && room.DimmerSettings == null)
        {
            var presetOne = new RoomDimmerPreset
            {
                RoomId = room.Id,
                PresetId = 1,
                BackgroundOnly = false,
                Color = "",
                Intensity = 255
            };

            var presetTwo = new RoomDimmerPreset
            {
                RoomId = room.Id,
                PresetId = 2,
                BackgroundOnly = false,
                Color = "",
                Intensity = 255
            };

            var presetThree = new RoomDimmerPreset
            {
                RoomId = room.Id,
                PresetId = 3,
                BackgroundOnly = false,
                Color = "",
                Intensity = 255
            };
            
            room.DimmerSettings = new RoomDimmerSettings
            {
                RoomId = room.Id,
                Enabled = false,
                PresetId = 1
            };

            dbContext.RoomDimmerPresets.Add(presetOne);
            dbContext.RoomDimmerPresets.Add(presetTwo);
            dbContext.RoomDimmerPresets.Add(presetThree);
            dbContext.RoomDimmerSettings.Add(room.DimmerSettings);
        }
        
        var interactor = interactorRepository.GetInteractorForType(roomFurnitureItem.FurnitureItem.InteractionType);

        if (interactor != null)
        {
            await interactor.OnPlaceAsync(room, roomFurnitureItem, client.RoomUser);
        }

        dbContext.Entry(playerItem).State = EntityState.Deleted;
        dbContext.Entry(roomFurnitureItem).State = EntityState.Added;
        
        await dbContext.SaveChangesAsync();
    }
}