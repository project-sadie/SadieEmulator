using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserGoToHotelView)]
public class RoomUserGoToHotelViewEventHandler(IRoomRepository roomRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var player = client.Player;
        var lastRoomId = player.State.CurrentRoomId;
        
        if (lastRoomId != 0)
        {
            var lastRoom = await RoomHelpers.TryLoadRoomByIdAsync(lastRoomId,
                roomRepository,
                dbContextFactory,
                mapper);

            if (lastRoom != null && lastRoom.UserRepository.TryGetById(player.Player.Id, out var oldUser) && oldUser != null)
            {
                await lastRoom.UserRepository.TryRemoveAsync(oldUser.Player.Player.Id);
            }
        }
    }
}