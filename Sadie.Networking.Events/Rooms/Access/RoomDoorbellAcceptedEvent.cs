using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Rooms.Access;

public class RoomDoorbellAcceptedEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;
    private readonly ILogger<RoomDoorbellAcceptedEvent> _logger;
    private readonly IRoomUserFactory _roomUserFactory;

    public RoomDoorbellAcceptedEvent(IRoomRepository roomRepository, ILogger<RoomDoorbellAcceptedEvent> logger, IRoomUserFactory roomUserFactory)
    {
        _roomRepository = roomRepository;
        _logger = logger;
        _roomUserFactory = roomUserFactory;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var roomId = reader.ReadInteger();

        var (roomFound, room) = _roomRepository.TryGetRoomById(roomId);

        if (!roomFound || room == null)
        {
            return;
        }
        
        await PacketEventHelpers.EnterRoomAsync(client, room, _logger, _roomUserFactory);
    }
}