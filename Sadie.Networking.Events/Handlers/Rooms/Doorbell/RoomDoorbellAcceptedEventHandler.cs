using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Doorbell;

[PacketId(EventHandlerIds.RoomDoorbellAccepted)]
public class RoomDoorbellAcceptedEventHandler(
    RoomDoorbellAcceptedEventParser eventParser,
    RoomRepository roomRepository,
    ILogger<RoomDoorbellAcceptedEventHandler> logger,
    RoomUserFactory roomUserFactory)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var room = roomRepository.TryGetRoomById(eventParser.RoomId);

        if (room == null)
        {
            return;
        }
        
        await RoomHelpersToClean.EnterRoomAsync(client, room, logger, roomUserFactory);
    }
}