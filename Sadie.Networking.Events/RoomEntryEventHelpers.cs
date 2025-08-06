using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Db;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Game.Players;
using Sadie.Enums.Miscellaneous;
using Sadie.Networking.Client;
using Sadie.Networking.Writers.Players;
using Sadie.Networking.Writers.Rooms;
using Serilog;

namespace Sadie.Networking.Events;

public static class RoomEntryEventHelpers
{
    public static async Task GenericEnterRoomAsync(
        INetworkClient client, 
        IRoomLogic room, 
        IRoomUserFactory roomUserFactory,
        IDbContextFactory<SadieDbContext> dbContextFactory,
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
            entryDirection = (int) teleport.Direction;
            
            player.State.Teleport = null;
        }
        
        var roomUser = RoomHelpers.CreateUserForEntry(roomUserFactory, room, player, entryPoint, (HDirection) entryDirection);
        roomUser.ApplyFlatCtrlStatus();
        
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
        
        if (!room.UserRepository.TryAdd(roomUser))
        {
            Log.Error($"Failed to add user {player.Id} to room {room.Id}");
            return;
        }
        
        player.State.CurrentRoomId = room.Id;

        room.TileMap.AddUnitToMap(entryPoint, roomUser);
        
        client.RoomUser = roomUser;
        
        await SendRoomEntryPacketsToUserAsync(client, room);
        
        var friends = player
            .GetMergedFriendships();
        
        await playerHelperService.UpdatePlayerStatusForFriendsAsync(
            player,
            friends, 
            true, 
            true,
            playerRepository);
        
        await RoomHelpers.CreateRoomVisitForPlayerAsync(player, room.Id, dbContextFactory);
        
        await Task.Delay(100);
        
        foreach (var user in room.UserRepository.GetAll())
        {
            if (user.Player.Ignores.Any(pi => pi.TargetPlayerId == player.Id))
            {
                await user.Player.NetworkObject!.WriteToStreamAsync(
                    new PlayerIgnoreStateWriter
                    {
                        State = (int) PlayerIgnoreState.Ignored,
                        Username = player.Username,
                    });
            }
            
            if (player.Ignores.Any(pi => pi.TargetPlayerId == user.Player.Id))
            {
                await player.NetworkObject!.WriteToStreamAsync(
                    new PlayerIgnoreStateWriter
                    {
                        State = (int) PlayerIgnoreState.Ignored,
                        Username = user.Player.Username,
                    });
            }
        }
            
        var matchingWiredTriggers = room.FurnitureItems
            .Where(x =>
                x.FurnitureItem.InteractionType ==
                FurnitureItemInteractionType.WiredTriggerEnterRoom)
            .ToList();

        foreach (var trigger in matchingWiredTriggers)
        {
            await wiredService.RunTriggerForRoomAsync(room, trigger, roomUser);
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
            AdId = -1,
            OwnerId = -1,
            OwnerUsername = "",
            FlatId = 0,
            Type = 0,
            Name = "",
            Description = "",
            Unknown8 = 0,
            Unknown9 = 0,
            CategoryId = 0
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
}