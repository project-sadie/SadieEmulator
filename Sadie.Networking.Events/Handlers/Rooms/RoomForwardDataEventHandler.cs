using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms;

public class RoomForwardDataEventHandler(RoomForwardDataEventParser eventParser, IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomForwardData;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var (found, room) = await roomRepository.TryLoadRoomByIdAsync(eventParser.RoomId);
        
        if (!found || room == null)
        {
            return;
        }

        if (client.Player?.Data == null)
        {
            return;
        }
        
        var unknown3 = !(eventParser.Unknown1 == 0 && eventParser.Unknown2 == 1);
        var isOwner = room.OwnerId == client.Player.Data.Id;
        
        await client.WriteToStreamAsync(new RoomForwardDataWriter(room!, true, unknown3, isOwner).GetAllBytes());
    }
}