using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users.Chat;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerId.RoomUserStopTyping)]
public class RoomUserStopTypingEventHandler() : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var roomUser = client.RoomUser;
        
        if (roomUser == null)
        {
            return;
        }

        await roomUser.Room.UserRepository.BroadcastDataAsync(new RoomUserTypingWriter
        {
            UserId = roomUser.Player.Id,
            IsTyping = false
        });
    }
}