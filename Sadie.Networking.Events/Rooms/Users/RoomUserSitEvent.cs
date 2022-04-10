using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Rooms.Users;

public class RoomUserSitEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomUserSitEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out _, out var roomUser))
        {
            return Task.CompletedTask;
        }
        
        roomUser.StatusMap[RoomUserStatus.Sit] = 0.5 + "";
        return Task.CompletedTask;
    }
}