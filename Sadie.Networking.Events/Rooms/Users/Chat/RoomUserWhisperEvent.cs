using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users.Chat;
using Sadie.Shared.Unsorted.Game;

namespace Sadie.Networking.Events.Rooms.Users.Chat;

public class RoomUserWhisperEvent(IRoomRepository roomRepository, RoomConstants roomConstants)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        var whisperData = reader.ReadString().Split(" ");
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