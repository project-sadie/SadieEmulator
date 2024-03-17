using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Rooms.Access;

public class RoomDoorbellAcceptedEvent(
    IRoomRepository roomRepository,
    ILogger<RoomDoorbellAcceptedEvent> logger,
    IRoomUserFactory roomUserFactory)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var roomId = reader.ReadInteger();

        var (roomFound, room) = roomRepository.TryGetRoomById(roomId);

        if (!roomFound || room == null)
        {
            return;
        }
        
        await PacketEventHelpers.EnterRoomAsync(client, room, logger, roomUserFactory);
    }
}