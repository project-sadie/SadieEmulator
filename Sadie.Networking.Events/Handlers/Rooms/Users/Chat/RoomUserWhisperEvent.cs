using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users.Chat;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users.Chat;
using Sadie.Shared.Unsorted.Game;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

public class RoomUserWhisperEvent(
    RoomUserWhisperParser parser,
    IRoomRepository roomRepository, 
    RoomConstants roomConstants)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        var whisperData = parser.Data.Split(" ");
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

        var chatMessage = new RoomChatMessage(roomUser, whisperMessage, room, (ChatBubble) reader.ReadInteger(), 0, RoomChatMessageType.Whisper);

        await roomUser.OnTalkAsync(chatMessage);
        
        var packetBytes = new RoomUserWhisperWriter(
            chatMessage.Sender.Id,
            chatMessage.Message,
            chatMessage.EmotionId,
            (int) chatMessage.Bubble).GetAllBytes();
        
        await roomUser.NetworkObject.WriteToStreamAsync(packetBytes);
        await targetUser.NetworkObject.WriteToStreamAsync(packetBytes);
    }
}