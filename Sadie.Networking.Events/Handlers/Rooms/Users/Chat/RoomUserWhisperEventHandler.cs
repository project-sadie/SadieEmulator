using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerIds.RoomUserWhisper)]
public class RoomUserWhisperEventHandler(
    RoomUserWhisperEventParser eventParser,
    RoomRepository roomRepository, 
    ServerRoomConstants roomConstants,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        var whisperData = eventParser.Data.Split(" ");
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
            ChatBubbleId = eventParser.Bubble,
            EmotionId = 0,
            TypeId = RoomChatMessageType.Whisper,
            CreatedAt = DateTime.Now
        };

        var packetBytes = new RoomUserWhisperWriter
        {
            SenderId = chatMessage.PlayerId,
            Message = chatMessage.Message,
            EmotionId = chatMessage.EmotionId,
            Bubble = (int)chatMessage.ChatBubbleId
        };
        
        await roomUser.NetworkObject.WriteToStreamAsync(packetBytes);
        await targetUser.NetworkObject.WriteToStreamAsync(packetBytes);
        
        roomUser.UpdateLastAction();
        
        room.ChatMessages.Add(chatMessage);

        dbContext.Add(chatMessage);
        await dbContext.SaveChangesAsync();
    }
}