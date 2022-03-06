using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared;

namespace Sadie.Networking.Events.Rooms.Users.Chat;

public class RoomUserChatEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomUserChatEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var text = reader.ReadString();

        if (string.IsNullOrEmpty(text) || text.Length > SadieConstants.MaxChatMessageLength)
        {
            return;
        }

        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        var bubbleId = reader.ReadInt();
        roomUser.ChatBubble = (RoomChatBubble) bubbleId;

        var message = new RoomChatMessage(roomUser!, text, room!, (int) roomUser.ChatBubble, 1);
        await room!.UserRepository.BroadcastDataToUsersAsync(new RoomUserChatWriter(message).GetAllBytes());
    }
}