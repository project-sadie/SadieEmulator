using Sadie.API;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Db.Models.Constants;
using Sadie.Db.Models.Players;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Game.Players;
using Sadie.Enums.Game.Rooms;
using Sadie.Enums.Game.Rooms.Furniture;
using Sadie.Enums.Miscellaneous;
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
using Sadie.Networking.Writers.Players.Subscriptions;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Helpers;
using RoomChatMessage = Sadie.Db.Models.Rooms.Chat.RoomChatMessage;

namespace Sadie.Networking.Events;

public static class NetworkPacketEventHelpers
{
    public static async Task SendPlayerSubscriptionPacketsAsync(IPlayerLogic player)
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
    
    public static async Task SendLoginPacketsToPlayerAsync(INetworkObject networkObject, IPlayerLogic player)
    {
        var playerData = player.Data;
        var playerSubscriptions = player.Subscriptions;

        await networkObject.WriteToStreamAsync(new NoobnessLevelWriter
        {
            Level = 1
        });

        if (playerData.HomeRoomId != null)
        {
            await networkObject.WriteToStreamAsync(new PlayerHomeRoomWriter
            {
                HomeRoom = playerData.HomeRoomId.Value,
                RoomIdToEnter = playerData.HomeRoomId.Value
            });
        }

        await networkObject.WriteToStreamAsync(new PlayerEffectListWriter
        {
            Effects = []
        });

        await networkObject.WriteToStreamAsync(new PlayerClothingListWriter
        {
            SetIds = [],
            FurnitureNames = []
        });

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

        if (player.HasPermission(PlayerPermissionName.Moderator))
        {
            await networkObject.WriteToStreamAsync(new ModToolsWriter
            {
                Issues = [],
                MessageTemplates = [],
                Unknown3 = 0,
                CallForHelpPermission = true,
                ChatLogsPermission = true,
                AlertPermission = true,
                KickPermission = true,
                BanPermission = true,
                RoomAlertPermission = true,
                RoomKickPermission = true,
                RoomMessageTemplates = []
            });
        }
    }
    
    internal static bool TryResolveRoomObjectsForClient(
        IRoomRepository roomRepository, 
        INetworkClient client, out IRoomLogic room, out IRoomUser user)
    {
        var player = client.Player;
        
        if (player == null)
        {
            room = null!; user = null!;
            return false;
        }
        
        var roomId = player.State.CurrentRoomId;
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

    public static async Task SendFurniturePlacementErrorAsync(INetworkObject client, RoomFurniturePlacementError error)
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

    private static async Task ShowCommandsAsync(
        IRoomChatCommandRepository commandRepository,
        INetworkClient client)
    {
        var commands = commandRepository.GetRegisteredCommands();
            
        await client.WriteToStreamAsync(new PlayerAlertWriter
        {
            Message = string.Join(Environment.NewLine, commands.Select(BuildCommandLine))
        });
    }

    private static string BuildCommandLine(IRoomChatCommand command)
    {
        var parameters = command.Parameters.Count != 0
            ? " " + string.Join(" ", command.Parameters.Select(p => $"[{p}]"))
            : "";

        return $":{command.Trigger}{parameters} - {command.Description}";
    }

    public static async Task OnChatMessageAsync(
        INetworkClient client,
        string message,
        bool shouting,
        ServerRoomConstants roomConstants,
        IRoomRepository roomRepository,
        IRoomChatCommandRepository commandRepository,
        ChatBubble bubble,
        IRoomWiredService wiredService,
        IRoomHelperService roomHelperService)
    {
        if (message.Length >= 9 && message[..9] == ":commands")
        {

            if (
                TryResolveRoomObjectsForClient(roomRepository, client, out var room2, out var roomUser2))
            {
                await roomUser2.Room.UserRepository.BroadcastDataAsync(new RoomUserEffectWriter
                {
                    UserId = roomUser2.Player.Id,
                    EffectId = new Random().Next(1, 100),
                    DelayMs = 0
                });
            }
            
            await ShowCommandsAsync(commandRepository, client);
            return;
        }
        
        if (string.IsNullOrEmpty(message) || 
            message.Length > roomConstants.MaxChatMessageLength ||
            !TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser) ||
            (!shouting && message.StartsWith(':') && await ProcessCommandAsync(
                commandRepository,
                message,
                roomUser)))
        {
            return;
        }
        
        var chatMessage = new RoomChatMessage()
        {
            RoomId = room.Id,
            PlayerId = roomUser.Player.Id,
            Message = message,
            ChatBubbleId = bubble,
            EmotionId = roomHelperService.GetEmotionFromMessage(message),
            TypeId = RoomChatMessageType.Shout,
            CreatedAt = DateTime.Now
        };
        
        var excludedIds = room
            .UserRepository
            .GetAll()
            .Where(x =>
                x.Player.Ignores.Any(pi => pi.TargetPlayerId == roomUser.Player.Id))
            .Select(x => x.Player.Id)
            .ToList();

        if (shouting)
        {
            var writer = new RoomUserShoutWriter
            {
                SenderId = roomUser.Player.Id,
                Message = message,
                EmotionId = (int) roomHelperService.GetEmotionFromMessage(message),
                ChatBubbleId = (int)bubble,
                Urls = [],
                MessageLength = message.Length
            };
            
            await room.UserRepository.BroadcastDataAsync(writer, excludedIds);
        }
        else
        {
            var writer = new RoomUserChatWriter
            {
                SenderId = roomUser.Player.Id,
                Message = message,
                EmotionId = (int) roomHelperService.GetEmotionFromMessage(message),
                ChatBubbleId = (int)bubble,
                Urls = [],
                MessageLength = message.Length
            };
            
            await room.UserRepository.BroadcastDataAsync(writer, excludedIds);
        }
        
        room.ChatMessages.Add(chatMessage);

        var triggers = wiredService.GetTriggers(
            FurnitureItemInteractionType.WiredTriggerSaysSomething, room.FurnitureItems, message);
        
        foreach (var trigger in triggers)
        {
            await wiredService.RunTriggerForRoomAsync(room, trigger, roomUser);
        }
    }

    private static async Task<bool> ProcessCommandAsync(
        IRoomChatCommandRepository commandRepository,
        string message,
        IRoomUser roomUser)
    {
        var triggerWord = message.Split(" ")[0][1..].ToLower();
        var command = commandRepository.TryGetCommandByTriggerWord(triggerWord);
        
        if (command == null)
        {
            return false;
        }
        
        var roomOwner = roomUser.ControllerLevel == RoomControllerLevel.Owner;

        if (command.BypassPermissionCheckIfRoomOwner && !roomOwner)
        {
            return false;
        }

        if (!command.PermissionsRequired.All(x => roomUser.Player.HasPermission(x)))
        {
            return false;
        }

        var parameterQueue = new Queue<string>(message.Split(" ").Skip(1));
        var reader = new RoomChatCommandParameterReader(parameterQueue);
        
        await command.ExecuteAsync(roomUser, reader);
        
        return true;
    }
    
    public static Dictionary<int, long> GetPlayerCurrencyMapFromData(PlayerData playerData)
    {
        return new Dictionary<int, long>
        {
            {0, playerData.PixelBalance},
            {1, 0}, // snowflakes
            {2, 0}, // hearts
            {3, 0}, // gift points
            {4, 0}, // shells
            {5, playerData.SeasonalBalance},
            {101, 0}, // snowflakes
            {102, 0}, // unknown
            {103, playerData.GotwPoints},
            {104, 0}, // unknown
            {105, 0} // unknown
        };
    }
}