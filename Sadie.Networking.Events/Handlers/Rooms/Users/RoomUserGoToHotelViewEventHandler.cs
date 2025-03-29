using AutoMapper;
using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserGoToHotelView)]
public class RoomUserGoToHotelViewEventHandler(IRoomRepository roomRepository,
    SadieContext dbContext,
    IMapper mapper) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var player = client.Player;
        var lastRoomId = player.State.CurrentRoomId;
        
        if (lastRoomId != 0)
        {
            var lastRoom = await Game.Rooms.RoomHelpers.TryLoadRoomByIdAsync(lastRoomId,
                roomRepository,
                dbContext,
                mapper);

            if (lastRoom != null && lastRoom.UserRepository.TryGetById(player.Id, out var oldUser) && oldUser != null)
            {
                await lastRoom.UserRepository.TryRemoveAsync(oldUser.Id);
            }
        }
    }
}