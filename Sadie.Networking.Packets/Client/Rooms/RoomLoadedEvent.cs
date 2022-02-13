using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Rooms;
using Sadie.Shared;

namespace Sadie.Networking.Packets.Client.Rooms;

public class RoomLoadedEvent : INetworkPacketEvent
{
    private readonly ILogger<RoomLoadedEvent> _logger;
    private readonly IRoomRepository _roomRepository;

    public RoomLoadedEvent(ILogger<RoomLoadedEvent> logger, IRoomRepository roomRepository)
    {
        _logger = logger;
        _roomRepository = roomRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;
        var (roomId, password) = (reader.ReadInt(), reader.ReadString());
        var (found, room) = await _roomRepository.TryLoadRoomByIdAsync(roomId);

        if (!found || room == null)
        {
            _logger.LogError($"Failed to load room {roomId}");
            return;
        }

        player.LastRoomLoaded = roomId;

        if (!room.UserRepository.TryAdd(RoomUserFactory.Create(player.Id, room.Layout.DoorPoint, room.Layout.DoorDirection)))
        {
            _logger.LogError($"Failed to add user {player.Id} to room {roomId}");
            return;
        }
        
        await client.WriteToStreamAsync(new RoomLoadedWriter().GetAllBytes());
        await client.WriteToStreamAsync(new RoomDataWriter(roomId, room.Layout.Name).GetAllBytes());
        await client.WriteToStreamAsync(new RoomPaintWriter("landscape", "0.0").GetAllBytes());
    }
}