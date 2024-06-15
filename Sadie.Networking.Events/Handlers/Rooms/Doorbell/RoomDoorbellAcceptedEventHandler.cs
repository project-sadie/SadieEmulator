using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Doorbell;

[PacketId(EventHandlerIds.RoomDoorbellAccepted)]
public class RoomDoorbellAcceptedEventHandler(
    RoomRepository roomRepository,
    ILogger<RoomDoorbellAcceptedEventHandler> logger,
    RoomUserFactory roomUserFactory,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public int RoomId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var room = roomRepository.TryGetRoomById(RoomId);

        if (room == null)
        {
            return;
        }
        
        await RoomHelpersDirty.EnterRoomAsync(client, room, logger, roomUserFactory, dbContext);
    }
}