using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Rooms;

public class RoomForwardDataEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomForwardDataEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var roomId = reader.ReadInteger();

        var (found, room) = await _roomRepository.TryLoadRoomByIdAsync(roomId);
        
        if (!found)
        {
            return;
        }
        
        var unknown1 = reader.ReadInteger();
        var unknown2 = reader.ReadInteger();
        var unknown3 = !(unknown1 == 0 && unknown2 == 1);
        var isOwner = room.OwnerId == client.Player.Data.Id;
        
        await client.WriteToStreamAsync(new RoomForwardDataWriter(room!, true, unknown3, isOwner).GetAllBytes());
    }
}