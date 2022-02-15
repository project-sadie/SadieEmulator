using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Rooms.Users;

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

        if (string.IsNullOrEmpty(text) || text.Length > 100)
        {
            return;
        }

        var roomId = client.Player.LastRoomLoaded;
        var (result, room) = _roomRepository.TryGetRoomById(roomId);

        if (!result || room == null || !room.UserRepository.TryGet(client.Player.Id, out var user) || user == null)
        {
            return;
        }

        var bubbleColor = reader.ReadInt();
        var message = new RoomChatMessage(user, text, room, bubbleColor, 1);
        
        await room.UserRepository.BroadcastDataToUsersAsync(new RoomUserChatWriter(message).GetAllBytes());
    }
}