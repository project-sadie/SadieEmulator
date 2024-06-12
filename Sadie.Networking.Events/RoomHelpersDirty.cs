using System.Drawing;
using Microsoft.Extensions.Logging;
using Sadie.Database.Models.Catalog;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms;
using Sadie.Enums.Game.Rooms;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Writers.Players.Purse;
using Sadie.Networking.Writers.Rooms;
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

        playerData.CreditBalance -= costInCredits;

        if (item.CostPointsType == 0)
        {
            playerData.PixelBalance -= costInPoints;
        }
        else
        {
            playerData.SeasonalBalance -= costInPoints;
        }
        
        await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter
        {
            Credits = playerData.CreditBalance
        });
        
        await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter
        {
            PixelBalance = playerData.PixelBalance,
            SeasonalBalance = playerData.SeasonalBalance,
            GotwPoints = playerData.GotwPoints
        });

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

            async void OnReachedGoal()
            {
                teleport.MetaData = "0";
                await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, teleport);
            }

            roomUser.WalkToPoint(squareInFront, OnReachedGoal);
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

        playerData.CreditBalance -= costInCredits;

        if (offer.CostPointsType == 0)
        {
            playerData.PixelBalance -= costInPoints;
        }
        else
        {
            playerData.SeasonalBalance -= costInPoints;
        }
        
        await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter
        {
            Credits = playerData.CreditBalance
        });
        
        await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter
        {
            PixelBalance = playerData.PixelBalance,
            SeasonalBalance = playerData.SeasonalBalance,
            GotwPoints = playerData.GotwPoints
        });

        return true;
    }
}