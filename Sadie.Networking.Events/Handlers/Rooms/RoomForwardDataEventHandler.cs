using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerIds.RoomForwardData)]
public class RoomForwardDataEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var room = roomRepository.TryGetRoomById(RoomId);

        if (room == null)
        {
            return;
        }

        if (client.Player?.Data == null)
        {
            return;
        }
        
        var unknown3 = eventParser is not { Unknown1: 0, Unknown2: 1 };
        var isOwner = room.OwnerId == client.Player.Id;
        
        await client.WriteToStreamAsync(new  RoomForwardDataWriter
        {
            Room = room,
            RoomForward = true,
            EnterRoom = unknown3,
            IsOwner = isOwner
        });
    }
}