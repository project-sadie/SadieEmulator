using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Rooms.Users;
using Sadie.Shared;

namespace Sadie.Networking.Packets.Client.Rooms.Users.Chat;

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

        if (!PacketHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        var bubbleColor = reader.ReadInt();
        var message = new RoomChatMessage(roomUser!, text, room!, bubbleColor, 1);
        
        await room!.UserRepository.BroadcastDataToUsersAsync(new RoomUserChatWriter(message).GetAllBytes());
    }
}