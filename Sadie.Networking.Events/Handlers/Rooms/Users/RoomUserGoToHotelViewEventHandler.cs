using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserGoToHotelViewEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomUserGoToHotelView;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;
        var lastRoomId = player.CurrentRoomId;
        
        if (lastRoomId != 0)
        {
            var (foundLast, lastRoom) = await roomRepository.TryLoadRoomByIdAsync(lastRoomId);

            if (foundLast && lastRoom != null && lastRoom.UserRepository.TryGet(player.Id, out var oldUser) && oldUser != null)
            {
                await lastRoom.UserRepository.TryRemoveAsync(oldUser.Id, true);
            }
        }
    }
}