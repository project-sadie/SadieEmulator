using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Rooms;

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
        var (roomId, password) = (reader.ReadInt(), reader.ReadString());
        var (found, room) = await _roomRepository.TryLoadRoomByIdAsync(roomId);

        if (!found || room == null)
        {
            _logger.LogError($"Failed to load room with ID {roomId}");
            return;
        }

        client.Player.LastRoomLoaded = roomId;

        await client.WriteToStreamAsync(new RoomLoadedWriter().GetAllBytes());
        await client.WriteToStreamAsync(new RoomDataWriter(roomId, room.Layout.Name).GetAllBytes());
        await client.WriteToStreamAsync(new RoomPaintWriter("floor", "0.0").GetAllBytes());
        await client.WriteToStreamAsync(new RoomPaintWriter("wallpaper", "0.0").GetAllBytes());
        await client.WriteToStreamAsync(new RoomPaintWriter("landscape", "0.0").GetAllBytes());
    }
}