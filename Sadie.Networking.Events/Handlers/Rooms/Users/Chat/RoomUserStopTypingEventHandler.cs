using Sadie.API.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users.Chat;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.Chat;

[PacketId(EventHandlerId.RoomUserStopTyping)]
public class RoomUserStopTypingEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
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
            UserId = roomUser.Id,
            IsTyping = false
        });
    }
}