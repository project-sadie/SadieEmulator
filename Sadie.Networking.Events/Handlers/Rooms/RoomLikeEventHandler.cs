using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Db.Models.Players;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerId.RoomLike)]
public class RoomLikeEventHandler(IRoomRepository roomRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!RoomContextResolver.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
        {
            return;
        }

        if (room.Room.OwnerId == client.Player.Player.Id || client.Player.Player.RoomLikes.FirstOrDefault(x => x.RoomId == room.Room.Id) != null)
        {
            return;
        }

        var roomLike = new PlayerRoomLikeDto()
        {
            PlayerId = client.Player.Player.Id,
            RoomId = room.Room.Id
        };
        
        client.Player.Player.RoomLikes.Add(roomLike);
        
        var roomLikeEntity = mapper.Map<PlayerRoomLike>(roomLike);
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.PlayerRoomLikes.Add(roomLikeEntity);
        await dbContext.SaveChangesAsync();
    }
}