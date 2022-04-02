using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Rooms.Users;

public class RoomUserDanceEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomUserDanceEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        var danceId = reader.ReadInt();
        await room.UserRepository.BroadcastDataAsync(new RoomUserDanceWriter(roomUser!.Id, danceId).GetAllBytes());
    }
}