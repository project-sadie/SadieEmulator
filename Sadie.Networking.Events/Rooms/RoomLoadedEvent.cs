using Microsoft.Extensions.Logging;
using Sadie.Game.Players.Avatar;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Rooms;

namespace Sadie.Networking.Packets.Client.Rooms;

public class RoomLoadedEvent : INetworkPacketEvent
{
    private readonly ILogger<RoomLoadedEvent> _logger;
    private readonly IRoomRepository _roomRepository;
    private readonly IRoomUserFactory _roomUserFactory;

    public RoomLoadedEvent(ILogger<RoomLoadedEvent> logger, IRoomRepository roomRepository, IRoomUserFactory roomUserFactory)
    {
        _logger = logger;
        _roomRepository = roomRepository;
        _roomUserFactory = roomUserFactory;
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

        var roomUser = _roomUserFactory.Create(
            client,
            player.Id,
            room.Layout.DoorPoint,
            room.Layout.DoorDirection,
            room.Layout.DoorDirection,
            player.Username,
            player.Motto,
            player.FigureCode,
            player.Gender == PlayerAvatarGender.Male ? "M" : "F",
            player.AchievementScore);

        if (!room.UserRepository.TryAdd(roomUser))
        {
            _logger.LogError($"Failed to add user {player.Id} to room {roomId}");
            return;
        }

        client.RoomUser = roomUser;
        
        await client.WriteToStreamAsync(new RoomLoadedWriter().GetAllBytes());
        await client.WriteToStreamAsync(new RoomDataWriter(roomId, room.Layout.Name).GetAllBytes());
        await client.WriteToStreamAsync(new RoomPaintWriter("landscape", "0.0").GetAllBytes());
    }
}