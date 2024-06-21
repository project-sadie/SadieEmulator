using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserSit)]
public class RoomUserSitEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
        {
            return Task.CompletedTask;
        }
        
        if ((int) roomUser.Direction % 2 != 0)
        {
            return Task.CompletedTask;
        }
        
        roomUser.AddStatus(RoomUserStatus.Sit, 0.5.ToString());
        
        return Task.CompletedTask;
    }
}