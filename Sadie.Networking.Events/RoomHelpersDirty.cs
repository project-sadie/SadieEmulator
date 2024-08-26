using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Services;
using Sadie.Database;
using Sadie.Database.Models.Catalog;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Game.Players;
using Sadie.Enums.Game.Rooms;
using Sadie.Enums.Game.Rooms.Furniture;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms.Bots;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Players.Purse;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Furniture;
using Serilog;

namespace Sadie.Networking.Events;

public static class RoomHelpersDirty
{
    private static RoomControllerLevel GetControllerLevelForUser(IRoom room, IPlayerLogic player)
    {
        var controllerLevel = RoomControllerLevel.None;
        
        if (room.PlayerRights.FirstOrDefault(x => x.PlayerId == player.Id) != null)
        {
            controllerLevel = RoomControllerLevel.Rights;
        }

        if (room.OwnerId == player.Id)
        {
            controllerLevel = RoomControllerLevel.Owner;
        }

        if (player.HasPermission("any_room_owner"))
        {
            controllerLevel = RoomControllerLevel.Owner;
        }

        if (player.HasPermission("moderator"))
        {
            controllerLevel = RoomControllerLevel.Moderator;
        }

        if (player.HasPermission("any_room_rights"))
        {
            controllerLevel = RoomControllerLevel.Rights;
        }

        return controllerLevel;
    }

    private static async Task CreateRoomVisitForPlayerAsync(
        IPlayerLogic player, 
        int roomId, 
        SadieContext dbContext)
    {
        var roomVisit = new PlayerRoomVisit
        {
            PlayerId = player.Id,
            RoomId = roomId,
            CreatedAt = DateTime.Now
        };
        
        player.RoomVisits.Add(roomVisit);

        dbContext.PlayerRoomVisits.Add(roomVisit);
        await dbContext.SaveChangesAsync();
    }

    public static RoomBot CreateBot(
        int id, 
        IRoomLogic room, 
        Point point,
        RoomBotFactory roomBotFactory)
    {
        return roomBotFactory.Create(room, id, point);
    }

    private static RoomUser CreateUserForEntry(
        RoomUserFactory roomUserFactory, 
        IRoomLogic room, 
        IPlayerLogic player,
        Point spawnPoint,
        HDirection direction)
    {
        var z = room.TileMap.ZMap[spawnPoint.Y, spawnPoint.X];
        
        return roomUserFactory.Create(
            room,
            player.NetworkObject,
            player.Id,
            spawnPoint,
            z,
            direction,
            direction,
            player,
            GetControllerLevelForUser(room, player));
    }
    
    public static async Task AfterEnterRoomAsync(
        INetworkClient client, 
        IRoomLogic room, 
        RoomUserFactory roomUserFactory,
        SadieContext dbContext,
        IPlayerRepository playerRepository,
        IRoomTileMapHelperService tileMapHelperService,
        IPlayerHelperService playerHelperService,
        IRoomFurnitureItemHelperService roomFurnitureItemHelperService,
        IRoomWiredService wiredService)
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
            Log.Error($"Failed to add user {player.Id} to room {room.Id}");
            return;
        }
        
        if (teleport != null)
        {
            var squareInFront = tileMapHelperService
                .GetPointInFront(teleport.PositionX, teleport.PositionY, teleport.Direction);

            if (!room.TileMap.UsersAtPoint(squareInFront))
            {
                roomUser.WalkToPoint(squareInFront);
            }
            
            await Task.Factory.StartNew(async () =>
            {
                await Task.Delay(800);
                await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(room, teleport, "0");
            });
        }

        if (player.State.CurrentRoomId == 0)
        {
            var friends = player
                .GetMergedFriendships()
                .Where(x => x.Status == PlayerFriendshipStatus.Accepted)
                .ToList();
        
            await playerHelperService.UpdatePlayerStatusForFriendsAsync(
                player,
                friends, 
                true, 
                true,
                playerRepository);
        }
        
        player.State.CurrentRoomId = room.Id;

        room.TileMap.AddUnitToMap(entryPoint, roomUser);
        roomUser.ApplyFlatCtrlStatus();
        
        client.RoomUser = roomUser;
        
        await SendRoomEntryPacketsToUserAsync(client, room);
        await CreateRoomVisitForPlayerAsync(player, room.Id, dbContext);
        
        var matchingWiredTriggers = room.FurnitureItems.Where(x =>
            x.FurnitureItem!.InteractionType == FurnitureItemInteractionType.WiredTriggerEnterRoom);
        
        foreach (var trigger in matchingWiredTriggers)
        {
            await wiredService.RunTriggerForRoomAsync(room, trigger);
        }
    }

    private static async Task SendRoomEntryPacketsToUserAsync(INetworkClient client, IRoomLogic room)
    {
        var player = client.Player;
        var roomUser = client.RoomUser;
        var canLikeRoom = player.RoomLikes.FirstOrDefault(x => x.RoomId == room.Id) == null;
        
        await client.WriteToStreamAsync(new RoomDataWriter
        {
            LayoutName = room.Layout.Name,
            RoomId = room.Id
        });

        if (room.PaintSettings?.FloorPaint != "0.0")
        {
            await client.WriteToStreamAsync(new RoomPaintWriter
            {
                Type = "floor",
                Value = room.PaintSettings?.FloorPaint ?? "0.0"
            });
        }

        if (room.PaintSettings?.WallPaint != "0.0")
        {
            await client.WriteToStreamAsync(new RoomPaintWriter
            {
                Type = "wallpaper",
                Value = room.PaintSettings?.WallPaint ?? "0.0"
            });
        }
        
        await client.WriteToStreamAsync(new RoomPaintWriter
        {
            Type = "landscape",
            Value = room.PaintSettings?.LandscapePaint ?? "0.0"
        });
        
        await client.WriteToStreamAsync(new RoomScoreWriter
        {
            Score = room.PlayerLikes.Count,
            CanUpvote = canLikeRoom
        });
        
        await client.WriteToStreamAsync(new RoomPromotionWriter
        {
            Unknown1 = -1,
            Unknown2 = -1,
            Unknown3 = "",
            Unknown4 = 0,
            Unknown5 = 0,
            Unknown6 = "",
            Unknown7 = "",
            Unknown8 = 0,
            Unknown9 = 0,
            Unknown10 = 0
        });
        
        var owner = room.OwnerId == player.Id;
        
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
        IRoomLogic room, 
        INetworkClient client, 
        PlayerFurnitureItem playerItem, 
        int itemId,
        SadieContext dbContext,
        IRoomFurnitureItemInteractorRepository interactorRepository,
        IRoomTileMapHelperService tileMapHelperService,
        IRoomFurnitureItemHelperService roomFurnitureItemHelperService)
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

        if (!pointsForPlacement
            .All(x => tileMapHelperService.CanPlaceAt([new Point(x.X, x.Y)], room.TileMap)))
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            return;
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
            ItemId = itemId
        });
        
        
        var interactors = interactorRepository
            .GetInteractorsForType(roomFurnitureItem.FurnitureItem.InteractionType);

        foreach (var interactor in interactors)
        {
            await interactor.OnPlaceAsync(client.RoomUser.Room, roomFurnitureItem, client.RoomUser);
        }

        await room.UserRepository.BroadcastDataAsync(new RoomFloorItemPlacedWriter
        {
            Id = roomFurnitureItem.PlayerFurnitureItem.Id,
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

        dbContext.Entry(roomFurnitureItem.PlayerFurnitureItem).State = EntityState.Unchanged;
        dbContext.RoomFurnitureItems.Add(roomFurnitureItem);
        
        await dbContext.SaveChangesAsync();
    }

    public static async Task OnPlaceWallItemAsync(
        IReadOnlyList<string> placementData,
        IRoomLogic room,
        PlayerFurnitureItem playerItem,
        int itemId,
        INetworkClient client,
        SadieContext dbContext,
        IRoomFurnitureItemInteractorRepository interactorRepository)
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

        dbContext.Entry(roomFurnitureItem).State = EntityState.Added;
        await dbContext.SaveChangesAsync();
    }
}