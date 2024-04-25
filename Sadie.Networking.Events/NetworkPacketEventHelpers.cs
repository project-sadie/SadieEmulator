using System.Drawing;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Game.Players.RoomVisits;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Game.Rooms.Enums;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users.Chat;
using Sadie.Networking.Writers;
using Sadie.Networking.Writers.Generic;
using Sadie.Networking.Writers.Players.Purse;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Doorbell;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Events;

internal static class NetworkPacketEventHelpers
{
    internal static bool TryResolveRoomObjectsForClient(
        RoomRepository roomRepository, 
        INetworkClient client, out RoomLogic room, out IRoomUser user)
    {
        var player = client.Player;
        
        if (player == null)
        {
            room = null!;
            user = null!;
            
            return false;
        }
        
        var roomId = player.CurrentRoomId;
        var roomObject = roomRepository.TryGetRoomById(roomId);

        if (roomObject == null || client.RoomUser == null)
        {
            room = null!;
            user = null!;
            
            return false;
        }

        room = roomObject;
        user = client.RoomUser;
        
        return true;
    }

    public static async Task SendFurniturePlacementErrorAsync(INetworkObject client, FurniturePlacementError error)
    {
        await client.WriteToStreamAsync(new NotificationWriter(NotificationType.FurniturePlacementError,
            new Dictionary<string, string>
            {
                { "message", error.ToString() }
            }));
    }

    internal static async Task EnterRoomAsync<T>(
        INetworkClient client, 
        RoomLogic room, 
        ILogger<T> logger, 
        RoomUserFactory roomUserFactory)
    {
        var player = client.Player;
        var playerState = player.State;
        
        player.CurrentRoomId = room.Id;
        playerState.RoomVisits.Add(new PlayerRoomVisit(player.Id, room.Id));

        var controllerLevel = RoomControllerLevel.None;
        
        if (room.PlayerRights.FirstOrDefault(x => x.PlayerId == player.Id) != null)
        {
            controllerLevel = RoomControllerLevel.Rights;
        }

        if (room.OwnerId == player.Id)
        {
            controllerLevel = RoomControllerLevel.Owner;
        }

        var doorPoint = new Point(room.Layout.DoorX, room.Layout.DoorY);
        var z = 0; // TODO: Calculate this
        
        var roomUser = roomUserFactory.Create(
            room,
            player.NetworkObject,
            player.Id,
            doorPoint,
            z,
            room.Layout.DoorDirection,
            room.Layout.DoorDirection,
            player,
            controllerLevel);

        room.TileMap.AddUserToMap(doorPoint, roomUser);
        roomUser.ApplyFlatCtrlStatus();
        
        if (!room.UserRepository.TryAdd(roomUser))
        {
            logger.LogError($"Failed to add user {player.Id} to room {room.Id}");
            return;
        }
        
        client.RoomUser = roomUser;

        var canLikeRoom = player.RoomLikes.FirstOrDefault(x => x.RoomId == room.Id) == null;
        
        await client.WriteToStreamAsync(new RoomDataWriter(room.Id, room.Layout.Name));

        if (room.PaintSettings.FloorPaint != "0.0")
        {
            await client.WriteToStreamAsync(new RoomPaintWriter("floor", room.PaintSettings.FloorPaint));
        }

        if (room.PaintSettings.WallPaint != "0.0")
        {
            await client.WriteToStreamAsync(new RoomPaintWriter("wallpaper", room.PaintSettings.WallPaint));
        }
        
        await client.WriteToStreamAsync(new RoomPaintWriter("landscape", room.PaintSettings.LandscapePaint));
        
        await client.WriteToStreamAsync(new RoomScoreWriter(room.PlayerLikes.Count, canLikeRoom));
        await client.WriteToStreamAsync(new RoomPromotionWriter());

        var owner = room.OwnerId == player.Id;
        
        await client.WriteToStreamAsync(new RoomWallFloorSettingsWriter(
            room.Settings.HideWalls, 
            room.Settings.WallThickness, 
            room.Settings.FloorThickness));
        
        await client.WriteToStreamAsync(new RoomPaneWriter(room.Id, owner));
        await client.WriteToStreamAsync(new RoomRightsWriter(roomUser.ControllerLevel));
        
        if (owner)
        {
            await client.WriteToStreamAsync(new RoomOwnerWriter());
        }

        await client.WriteToStreamAsync(new RoomLoadedWriter());
    }

    public static async Task ProcessChatMessageAsync(
        INetworkClient client,
        RoomUserChatEventParser parser,
        bool shouting,
        ServerRoomConstants roomConstants,
        RoomRepository roomRepository,
        IRoomChatCommandRepository commandRepository,
        SadieContext dbContext)
    {
        var message = parser.Message;
        
        if (string.IsNullOrEmpty(message) || message.Length > roomConstants.MaxChatMessageLength)
        {
            return;
        }
        
        if (!TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        if (!shouting && message.StartsWith(":"))
        {
            var command = commandRepository.TryGetCommandByTriggerWord(message.Split(" ")[0][1..]);
            var roomOwner = room.OwnerId == roomUser.Id;
            
            if (command != null && 
                ((command.BypassPermissionCheckIfRoomOwner && roomOwner) || 
                 command.PermissionsRequired.All(x => roomUser.Player.HasPermission(x))))
            {
                await command.ExecuteAsync(roomUser!, message.Split(" ").Skip(1));
                return;
            }
        }

        var chatMessage = new RoomChatMessage
        {
            RoomId = room.Id,
            PlayerId = roomUser.Id,
            Message = message,
            ChatBubbleId = parser.Bubble,
            EmotionId = 0,
            TypeId = RoomChatMessageType.Shout,
            CreatedAt = DateTime.Now
        };

        await room!.UserRepository.BroadcastDataAsync(
            shouting ? 
            new RoomUserShoutWriter(chatMessage!, 0) : 
            new RoomUserChatWriter(chatMessage!, 0)
        );
        
        roomUser.UpdateLastAction();

        room.ChatMessages.Add(chatMessage);

        dbContext.Add(chatMessage);
        await dbContext.SaveChangesAsync();
    }

    public static async Task<bool> ValidateRoomAccessForClientAsync(INetworkClient client, RoomLogic room, string password)
    {
        var player = client.Player!;
        
        switch (room.Settings.AccessType)
        {
            case RoomAccessType.Password when password != room.Settings.Password:
                await client.WriteToStreamAsync(new GenericErrorWriter(GenericErrorCode.IncorrectRoomPassword));
                await client.WriteToStreamAsync(new PlayerHotelViewWriter());
                return false;
            case RoomAccessType.Doorbell:
            {
                var usersWithRights = room.UserRepository.GetAllWithRights();
                
                if (usersWithRights.Count < 1)
                {
                    await client.WriteToStreamAsync(new RoomDoorbellNoAnswerWriter(player.Username));
                }
                else
                {
                    foreach (var user in usersWithRights)
                    {
                        await user.NetworkObject.WriteToStreamAsync(new RoomDoorbellWriter(player.Username)
                            );
                    }

                    await client.WriteToStreamAsync(new RoomDoorbellWriter());
                }

                return false;
            }
        }

        return true;
    }

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
        
        await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter(playerData.CreditBalance));
        
        await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter(
            playerData.PixelBalance, 
            playerData.SeasonalBalance, 
            playerData.GotwPoints));

        return true;
    }
}