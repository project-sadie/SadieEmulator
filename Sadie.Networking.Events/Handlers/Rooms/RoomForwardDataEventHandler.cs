using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerIds.RoomForwardData)]
public class RoomForwardDataEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int RoomId { get; set; }
    public int EnterRoom { get; set; }
    public int ForwardRoom { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
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

        var isOwner = room.OwnerId == client.Player.Id;
        
        await client.WriteToStreamAsync(new  RoomForwardDataWriter
        {
            Room = room,
            RoomForward = true,
            EnterRoom = EnterRoom != 0 || ForwardRoom != 1,
            IsOwner = isOwner
        });
    }
}