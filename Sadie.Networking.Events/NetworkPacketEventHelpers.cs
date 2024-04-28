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
            room = null!; user = null!;
            return false;
        }
        
        var roomId = player.CurrentRoomId;
        var roomObject = roomRepository.TryGetRoomById(roomId);

        if (roomObject == null || client.RoomUser == null)
        {
            room = null!; user = null!;
            return false;
        }

        room = roomObject;
        user = client.RoomUser;
        
        return true;
    }

    public static async Task SendFurniturePlacementErrorAsync(INetworkObject client, FurniturePlacementError error)
    {
        await client.WriteToStreamAsync(new NotificationWriter
        {
            Type = (int) NotificationType.FurniturePlacementError,
            Messages = new Dictionary<string, string>
            {
                { "message", error.ToString() }
            }
        });
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
        
        if (string.IsNullOrEmpty(message) || 
            message.Length > roomConstants.MaxChatMessageLength ||
            !TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
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
                var parameters = message.Split(" ").Skip(1);
                await command.ExecuteAsync(roomUser, parameters);
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
            new RoomUserShoutWriter { Message = chatMessage, Unknown1 = 0 } : 
            new RoomUserChatWriter { Message = chatMessage, Unknown1 = 0 });
        
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
                await client.WriteToStreamAsync(new GenericErrorWriter
                {
                    ErrorCode = (int) GenericErrorCode.IncorrectRoomPassword
                });
                await client.WriteToStreamAsync(new RoomUserHotelView());
                return false;
            
            case RoomAccessType.Doorbell:
            {
                var usersWithRights = room.UserRepository.GetAllWithRights();

                if (usersWithRights.Count < 1)
                {
                    await client.WriteToStreamAsync(new RoomDoorbellNoAnswerWriter
                    {
                        Username = player.Username
                    });
                    
                    return false;
                }
                
                foreach (var user in usersWithRights)
                {
                    await user.NetworkObject.WriteToStreamAsync(new RoomDoorbellWriter
                    {
                        Username = player.Username
                    });
                    
                }

                await client.WriteToStreamAsync(new RoomDoorbellWriter
                {
                    Username = ""
                });
                
                return false;
            }
            case RoomAccessType.Open:
                break;
            case RoomAccessType.Invisible:
                break;
            default:
                throw new ArgumentOutOfRangeException();
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