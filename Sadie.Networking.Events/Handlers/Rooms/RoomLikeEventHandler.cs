using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Db.Models.Players;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerId.RoomLike)]
public class RoomLikeEventHandler(IRoomRepository roomRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
        {
            return;
        }

        if (room.OwnerId == client.Player.Id || client.Player.RoomLikes.FirstOrDefault(x => x.RoomId == room.Id) != null)
        {
            return;
        }
        
        client.Player.RoomLikes.Add(new PlayerRoomLike
        {
            PlayerId = client.Player.Id,
            RoomId = room.Id
        });
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.SaveChangesAsync();
    }
}