using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Rooms.Users;

public class RoomUserSignEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomUserSignEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return Task.CompletedTask;
        }

        roomUser!.StatusMap[RoomUserStatus.Sign] = reader.ReadInt().ToString();
        
        return Task.CompletedTask;
    }
}