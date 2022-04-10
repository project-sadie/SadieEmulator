using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Networking.Events.Rooms;

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
        
        if (player!.LastRoomLoaded != default)
        {
            var (foundLast, lastRoom) = await _roomRepository.TryLoadRoomByIdAsync(player.LastRoomLoaded);

            if (foundLast && lastRoom != null && lastRoom.UserRepository.TryGet(player.Id, out var oldUser) && oldUser != null)
            {
                await lastRoom.UserRepository.TryRemoveAsync(oldUser.Id);
            }
        }

        if (!found || room == null)
        {
            _logger.LogError($"Failed to load room {roomId}");
            return;
        }

        player.LastRoomLoaded = roomId;

        var avatarData = (IAvatarData) player;
        
        var roomUser = _roomUserFactory.Create(
            room,
            client,
            player.Id,
            room.Layout.DoorPoint,
            room.Layout.DoorDirection,
            room.Layout.DoorDirection,
            avatarData);

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