using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Packets.Writers.Rooms.Users.Chat;

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

        await roomUser.Room.BroadcastDataAsync(new RoomUserTypingWriter
        {
            UserId = roomUser.Player.Player.Id,
            IsTyping = false
        });
    }
}