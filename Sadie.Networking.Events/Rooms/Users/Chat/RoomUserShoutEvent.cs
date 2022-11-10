using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Game;

namespace Sadie.Networking.Events.Rooms.Users.Chat;

public class RoomUserShoutEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;
    private readonly RoomConstants _roomConstants;

    public RoomUserShoutEvent(IRoomRepository roomRepository, RoomConstants roomConstants)
    {
        _roomRepository = roomRepository;
        _roomConstants = roomConstants;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var message = reader.ReadString();
        
        if (string.IsNullOrEmpty(message) || message.Length > _roomConstants.MaxChatMessageLength)
        {
            return;
        }
        
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        var chatMessage = new RoomChatMessage(roomUser, message, room, (ChatBubble) reader.ReadInteger(), 0);
        room.ChatMessages.Add(chatMessage);
        
        await room!.UserRepository.BroadcastDataAsync(new RoomUserShoutWriter(chatMessage!, 0).GetAllBytes());
    }
}