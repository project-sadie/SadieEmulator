using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users.Chat;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerIds.RoomUserStopTyping)]
public class RoomUserStopTypingEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var roomUser = client.RoomUser;
        
        if (roomUser == null)
        {
            return;
        }

        roomUser.UpdateLastAction();
        
        await roomUser.Room.UserRepository.BroadcastDataAsync(new RoomUserTypingWriter
        {
            UserId = roomUser.Id,
            IsTyping = false
        });
    }
}