using AutoMapper;
using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerIds.RoomForwardData)]
public class RoomForwardDataEventHandler(RoomRepository roomRepository,
    SadieContext dbContext,
    IMapper mapper) : INetworkPacketEventHandler
{
    public int RoomId { get; set; }
    public int EnterRoom { get; set; }
    public int ForwardRoom { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var room = await Game.Rooms.RoomHelpersDirty.TryLoadRoomByIdAsync(
            RoomId, 
            roomRepository, 
            dbContext, 
            mapper);
        
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