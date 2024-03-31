using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Doorbell;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms.Doorbell;

public class RoomDoorbellAcceptedEventHandler(
    RoomDoorbellAcceptedEventParser eventParser,
    IRoomRepository roomRepository,
    ILogger<RoomDoorbellAcceptedEventHandler> logger,
    IRoomUserFactory roomUserFactory)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomDoorbellAccepted;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var (roomFound, room) = roomRepository.TryGetRoomById(eventParser.RoomId);

        if (!roomFound || room == null)
        {
            return;
        }
        
        await NetworkPacketEventHelpers.EnterRoomAsync(client, room, logger, roomUserFactory);
    }
}