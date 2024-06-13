using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Enums.Game.Rooms;
using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Writers.Generic;
using Sadie.Networking.Writers.Handshake;
using Sadie.Networking.Writers.Moderation;
using Sadie.Networking.Writers.Players;
using Sadie.Networking.Writers.Players.Clothing;
using Sadie.Networking.Writers.Players.Effects;
using Sadie.Networking.Writers.Players.Navigator;
using Sadie.Networking.Writers.Players.Other;
using Sadie.Networking.Writers.Players.Permission;
using Sadie.Networking.Writers.Players.Rooms;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game;
using Sadie.Shared.Unsorted.Networking;
using RoomHelpers = Sadie.Shared.Helpers.RoomHelpers;

namespace Sadie.Networking.Events;

public static class NetworkPacketEventHelpers
{
    public static async Task SendPlayerSubscriptionPacketsAsync(PlayerLogic player)
    {
        foreach (var playerSub in player.Subscriptions)
        {
            var tillExpire = playerSub.ExpiresAt - playerSub.CreatedAt;
            var daysLeft = (int)tillExpire.TotalDays;
            var minutesLeft = (int)tillExpire.TotalMinutes;
            var minutesSinceMod = (int)(DateTime.Now - player.State.LastSubscriptionModification).TotalMinutes;

            await player.NetworkObject.WriteToStreamAsync(new PlayerSubscriptionWriter
            {
                Name = playerSub.Subscription.Name.ToLower(),
                DaysLeft = daysLeft,
                MemberPeriods = 0,
                PeriodsSubscribedAhead = 0,
                ResponseType = 1,
                HasEverBeenMember = true,
                IsVip = true,
                PastClubDays = 0,
                PastVipDays = 0,
                MinutesTillExpire = minutesLeft,
                MinutesSinceModified = minutesSinceMod
            });

            player.State.LastSubscriptionModification = DateTime.Now;
        }
    }
    
    public static async Task SendLoginPacketsToPlayerAsync(INetworkObject networkObject, PlayerLogic player)
    {
        var playerData = player.Data;
        var playerSubscriptions = player.Subscriptions;

        await networkObject.WriteToStreamAsync(new SecureLoginWriter());

        await networkObject.WriteToStreamAsync(new NoobnessLevelWriter
        {
            Level = 1
        });

        await networkObject.WriteToStreamAsync(new PlayerHomeRoomWriter
        {
            HomeRoom = playerData.HomeRoomId,
            RoomIdToEnter = playerData.HomeRoomId
        });

        await networkObject.WriteToStreamAsync(new PlayerEffectListWriter
        {
            Effects = []
        });

        await networkObject.WriteToStreamAsync(new PlayerClothingListWriter());

        await networkObject.WriteToStreamAsync(new PlayerPermissionsWriter
        {
            Club = playerSubscriptions.Any(x => x.Subscription.Name == "HABBO_CLUB") ? 2 : 0,
            Rank = player.Roles.Count != 0 ? player.Roles.Max(x => x.Id) : 1,
            Ambassador = true
        });

        var navigatorSettingsWriter = new PlayerNavigatorSettingsWriter
        {
            NavigatorSettings = player.NavigatorSettings!
        };

        var statusWriter = new PlayerStatusWriter
        {
            IsOpen = true,
            IsShuttingDown = false,
            IsAuthentic = true
        };

        await networkObject.WriteToStreamAsync(navigatorSettingsWriter);
        await networkObject.WriteToStreamAsync(statusWriter);

        await networkObject.WriteToStreamAsync(new PlayerNotificationSettingsWriter
        {
            ShowNotifications = player.GameSettings.ShowNotifications
        });

        await networkObject.WriteToStreamAsync(new PlayerAchievementScoreWriter
        {
            AchievementScore = playerData.AchievementScore
        });

        if (player.HasPermission("moderation_tools"))
        {
            await networkObject.WriteToStreamAsync(new ModerationToolsWriter
            {
                Unknown1 = 0,
                Unknown2 = 0,
                Unknown3 = 0,
                Unknown4 = true,
                Unknown5 = true,
                Unknown6 = true,
                Unknown7 = true,
                Unknown8 = true,
                Unknown9 = true,
                Unknown10 = true,
                Unknown11 = 0
            });
        }
    }
    
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
        await client.WriteToStreamAsync(new BubbleAlertWriter
        {
            Key = EnumHelpers.GetEnumDescription(NotificationType.FurniturePlacementError)!,
            Messages = new Dictionary<string, string>
            {
                { "message", error.ToString() }
            }
        });
    }
    
    public static async Task OnChatMessageAsync(
        INetworkClient client,
        string message,
        bool shouting,
        ServerRoomConstants roomConstants,
        RoomRepository roomRepository,
        IRoomChatCommandRepository commandRepository,
        SadieContext dbContext,
        ChatBubble bubble)
    {
        if (string.IsNullOrEmpty(message) || 
            message.Length > roomConstants.MaxChatMessageLength ||
            !TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        if (!shouting && message.StartsWith(":"))
        {
            var command = commandRepository.TryGetCommandByTriggerWord(message.Split(" ")[0][1..]);
            var roomOwner = roomUser.ControllerLevel == RoomControllerLevel.Owner;
            
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
            ChatBubbleId = bubble,
            EmotionId = RoomHelpers.GetEmotionFromMessage(message),
            TypeId = RoomChatMessageType.Shout,
            CreatedAt = DateTime.Now
        };

        if (shouting)
        {
            var writer = new RoomUserShoutWriter
            {
                UserId = roomUser.Id,
                Message = message,
                EmotionId = (int) RoomHelpers.GetEmotionFromMessage(message),
                ChatBubbleId = (int)bubble,
                Unknown1 = 0
            };
            
            await room.UserRepository.BroadcastDataAsync(writer);
        }
        else
        {
            var writer = new RoomUserChatWriter
            {
                UserId = roomUser.Id,
                Message = message,
                EmotionId = (int) RoomHelpers.GetEmotionFromMessage(message),
                ChatBubbleId = (int)bubble,
                Unknown1 = 0
            };
            
            await room.UserRepository.BroadcastDataAsync(writer);
        }
        
        roomUser.UpdateLastAction();
        room.ChatMessages.Add(chatMessage);

        dbContext.Add(chatMessage);
        await dbContext.SaveChangesAsync();
    }
}