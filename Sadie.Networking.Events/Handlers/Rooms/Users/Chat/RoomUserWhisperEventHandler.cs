using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Services;
using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Enums.Game.Rooms;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms.Packets.Writers.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerId.RoomUserWhisper)]
public class RoomUserWhisperEventHandler(
    IRoomRepository roomRepository, 
    ServerRoomConstants roomConstants,
    IDbContextFactory<SadieContext> dbContextFactory,
    IRoomHelperService roomHelperService)
    : INetworkPacketEventHandler
{
    public required string Data { get; init; }
    public int Bubble { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        var whisperData = Data.Split(" ");
        var whisperUsername = whisperData.First();
        var whisperMessage = string.Join("", whisperData.Skip(1));

        if (!room.UserRepository.TryGetByUsername(whisperUsername, out var targetUser) || targetUser == null)
        {
            return;
        }
        
        if (string.IsNullOrEmpty(whisperMessage) || whisperMessage.Length > roomConstants.MaxChatMessageLength)
        {
            return;
        }

        var chatMessage = new RoomChatMessage
        {
            RoomId = room.Id,
            PlayerId = roomUser.Id,
            Message = whisperMessage,
            ChatBubbleId = (ChatBubble) Bubble,
            EmotionId = roomHelperService.GetEmotionFromMessage(whisperMessage),
            TypeId = RoomChatMessageType.Whisper,
            CreatedAt = DateTime.Now
        };

        var packetBytes = new RoomUserWhisperWriter
        {
            SenderId = chatMessage.PlayerId,
            Message = chatMessage.Message,
            EmotionId = (int) chatMessage.EmotionId,
            ChatBubbleId = Bubble,
            MessageLength = chatMessage.Message.Length,
            Urls = []
        };
        
        await roomUser.NetworkObject.WriteToStreamAsync(packetBytes);
        await targetUser.NetworkObject.WriteToStreamAsync(packetBytes);
        
        room.ChatMessages.Add(chatMessage);
    }
}