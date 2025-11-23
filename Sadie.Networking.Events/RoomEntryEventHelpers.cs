using System.Drawing;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Mapping;
using Sadie.API.Interfaces.Game.Rooms.Services;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Db;
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
        IRoomWiredService wiredService,
        IMapper mapper)
    {
        var player = client.Player;
        var entryPoint = new Point(room.Room.Layout.DoorX, room.Room.Layout.DoorY);
        var entryDirection = room.Room.Layout.DoorDirection;
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
            Log.Error($"Failed to add user {player.Id} to room {room.Room.Id}");
            return;
        }
        
        player.State.CurrentRoomId = room.Room.Id;

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
        
        await RoomHelpers.CreateRoomVisitForPlayerAsync(player, room.Room.Id, dbContextFactory, mapper);
        
        await Task.Delay(100);
        
        foreach (var user in room.UserRepository.GetAll())
        {
            if (user.Player.Ignores.Any(pi => pi.TargetPlayerId == player.Id))
            {
                await user.Player.NetworkObject!.WriteToStreamAsync(
                    new PlayerIgnoreStateWriter
                    {
                        State = (int) PlayerIgnoreState.Ignored,
                        Username = player.Username
                    });
            }
            
            if (player.Ignores.Any(pi => pi.TargetPlayerId == user.Player.Id))
            {
                await player.NetworkObject!.WriteToStreamAsync(
                    new PlayerIgnoreStateWriter
                    {
                        State = (int) PlayerIgnoreState.Ignored,
                        Username = user.Player.Username
                    });
            }
        }
            
        var matchingWiredTriggers = room.Room.FurnitureItems
            .Where(x =>
                x
                    .PlayerFurnitureItem
                    .FurnitureItem.InteractionType == FurnitureItemInteractionType.WiredTriggerEnterRoom)
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
        var canLikeRoom = player.RoomLikes.FirstOrDefault(x => x.RoomId == room.Room.Id) == null;
        
        await client.WriteToStreamAsync(new RoomDataWriter
        {
            LayoutName = room.Room.Layout.Name,
            RoomId = room.Room.Id
        });

        if (room.Room.PaintSettings?.FloorPaint != "0.0")
        {
            await client.WriteToStreamAsync(new RoomPaintWriter
            {
                Type = "floor",
                Value = room.Room.PaintSettings?.FloorPaint ?? "0.0"
            });
        }

        if (room.Room.PaintSettings?.WallPaint != "0.0")
        {
            await client.WriteToStreamAsync(new RoomPaintWriter
            {
                Type = "wallpaper",
                Value = room.Room.PaintSettings?.WallPaint ?? "0.0"
            });
        }
        
        await client.WriteToStreamAsync(new RoomPaintWriter
        {
            Type = "landscape",
            Value = room.Room.PaintSettings?.LandscapePaint ?? "0.0"
        });
        
        await client.WriteToStreamAsync(new RoomScoreWriter
        {
            Score = room.Room.PlayerLikes.Count,
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
        
        var owner = room.Room.OwnerId == player.Id;
        
        await client.WriteToStreamAsync(new RoomPaneWriter
        {
            RoomId = room.Room.Id,
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