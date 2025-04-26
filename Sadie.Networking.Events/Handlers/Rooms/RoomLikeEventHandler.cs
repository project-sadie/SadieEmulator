using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerId.RoomLike)]
public class RoomLikeEventHandler(IRoomRepository roomRepository,
    IDbContextFactory<SadieContext> dbContextFactory) : INetworkPacketEventHandler
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