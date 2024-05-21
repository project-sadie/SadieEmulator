using System.Drawing;
using Microsoft.Extensions.Logging;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Players;
using Sadie.Game.Players.RoomVisits;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Enums;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Writers.Players.Purse;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events;

public static class RoomHelpers
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
        player.State.RoomVisits.Add(new PlayerRoomVisit(player.Id, roomId));
        // TODO; add to context
    }

    private static RoomUser CreateUserForEntry(
        RoomUserFactory roomUserFactory, 
        RoomLogic room, 
        PlayerLogic player,
        Point spawnPoint)
    {
        var z = 0; // TODO: Calculate this
        
        return roomUserFactory.Create(
            room,
            player.NetworkObject,
            player.Id,
            spawnPoint,
            z,
            room.Layout.DoorDirection,
            room.Layout.DoorDirection,
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
        var doorPoint = new Point(room.Layout.DoorX, room.Layout.DoorY);
        var roomUser = CreateUserForEntry(roomUserFactory, room, player, doorPoint);
        
        if (!room.UserRepository.TryAdd(roomUser))
        {
            logger.LogError($"Failed to add user {player.Id} to room {room.Id}");
            return;
        }
        
        CreateRoomVisitForPlayer(player, room.Id);
        player.CurrentRoomId = room.Id;

        room.TileMap.AddUserToMap(doorPoint, roomUser);
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
}