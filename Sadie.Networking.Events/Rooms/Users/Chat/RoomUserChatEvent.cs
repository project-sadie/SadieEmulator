using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Game;

namespace Sadie.Networking.Events.Rooms.Users.Chat;

public class RoomUserChatEvent(IRoomRepository roomRepository, RoomConstants roomConstants)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var message = reader.ReadString();
        
        if (string.IsNullOrEmpty(message) || message.Length > roomConstants.MaxChatMessageLength)
        {
            return;
        }
        
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        var chatMessage = new RoomChatMessage(roomUser, message, room, (ChatBubble) reader.ReadInteger(), 0);
        room.ChatMessages.Add(chatMessage);
        
        await room!.UserRepository.BroadcastDataAsync(new RoomUserChatWriter(chatMessage, 0).GetAllBytes());
    }
}