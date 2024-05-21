using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Doorbell;

[PacketId(EventHandlerIds.RoomDoorbellAccepted)]
public class RoomDoorbellAcceptedEventHandler(
    RoomRepository roomRepository,
    ILogger<RoomDoorbellAcceptedEventHandler> logger,
    RoomUserFactory roomUserFactory)
    : INetworkPacketEventHandler
{
    public int RoomId { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var room = roomRepository.TryGetRoomById(RoomId);

        if (room == null)
        {
            return;
        }
        
        await RoomHelpers.EnterRoomAsync(client, room, logger, roomUserFactory);
    }
}