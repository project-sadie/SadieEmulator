using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms;

public class RoomForwardDataEvent(IRoomRepository roomRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var roomId = reader.ReadInteger();

        var (found, room) = await roomRepository.TryLoadRoomByIdAsync(roomId);
        
        if (!found || room == null)
        {
            return;
        }

        if (client.Player == null || client.Player.Data == null)
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