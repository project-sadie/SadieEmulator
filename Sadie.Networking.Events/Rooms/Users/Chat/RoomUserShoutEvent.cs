using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Chat;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Rooms.Users.Chat;

public class RoomUserShoutEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomUserShoutEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        if (roomUser != null && roomUser.TryCreateChatMessage(reader.ReadString(), (RoomChatBubble) reader.ReadInt(), out var chatMessage))
        {
            await room!.UserRepository.BroadcastDataAsync(new RoomUserShoutWriter(chatMessage!, 0).GetAllBytes());
        }
    }
}