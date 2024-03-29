using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms;

public class RoomForwardDataEvent(RoomForwardDataParser parser, IRoomRepository roomRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        var (found, room) = await roomRepository.TryLoadRoomByIdAsync(parser.RoomId);
        
        if (!found || room == null)
        {
            return;
        }

        if (client.Player?.Data == null)
        {
            return;
        }
        
        var unknown3 = !(parser.Unknown1 == 0 && parser.Unknown2 == 1);
        var isOwner = room.OwnerId == client.Player.Data.Id;
        
        await client.WriteToStreamAsync(new RoomForwardDataWriter(room!, true, unknown3, isOwner).GetAllBytes());
    }
}