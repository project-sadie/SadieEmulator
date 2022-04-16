using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Rooms.Users;

public class RoomUserGoToHotelViewEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomUserGoToHotelViewEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;
        var lastRoomId = player.Data.CurrentRoomId;
        
        if (lastRoomId != 0)
        {
            var (foundLast, lastRoom) = await _roomRepository.TryLoadRoomByIdAsync(lastRoomId);

            if (foundLast && lastRoom != null && lastRoom.UserRepository.TryGet(player.Data.Id, out var oldUser) && oldUser != null)
            {
                await lastRoom.UserRepository.TryRemoveAsync(oldUser.Id, true);
            }
        }
    }
}