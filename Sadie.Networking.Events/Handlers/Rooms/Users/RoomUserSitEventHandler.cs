using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserSitEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomUserSit;

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
        {
            return Task.CompletedTask;
        }
        
        roomUser.StatusMap[RoomUserStatus.Sit] = 0.5 + "";
        return Task.CompletedTask;
    }
}