using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Doorbell;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms.Doorbell;

public class RoomDoorbellAcceptedEvent(
    RoomDoorbellAcceptedParser parser,
    IRoomRepository roomRepository,
    ILogger<RoomDoorbellAcceptedEvent> logger,
    IRoomUserFactory roomUserFactory)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        var (roomFound, room) = roomRepository.TryGetRoomById(parser.RoomId);

        if (!roomFound || room == null)
        {
            return;
        }
        
        await PacketEventHelpers.EnterRoomAsync(client, room, logger, roomUserFactory);
    }
}