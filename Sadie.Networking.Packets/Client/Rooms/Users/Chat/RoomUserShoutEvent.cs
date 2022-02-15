using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Rooms.Users;

namespace Sadie.Networking.Packets.Client.Rooms.Users.Chat;

public class RoomUserShoutEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomUserShoutEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var text = reader.ReadString();

        if (text.Length > 100)
        {
            return Task.CompletedTask;
        }

        var roomId = client.Player.LastRoomLoaded;
        var (result, room) = _roomRepository.TryGetRoomById(roomId);

        if (!result || room == null || !room.UserRepository.TryGet(client.Player.Id, out var user) || user == null)
        {
            return Task.CompletedTask;
        }

        var bubbleColor = reader.ReadInt();
        var message = new RoomChatMessage(user, text, room, bubbleColor, 1);
        
        client.WriteToStreamAsync(new RoomUserShoutWriter(message).GetAllBytes());
        
        return Task.CompletedTask;
    }
}