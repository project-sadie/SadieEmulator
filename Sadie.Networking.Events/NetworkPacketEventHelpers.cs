using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Writers.Generic;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game;
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