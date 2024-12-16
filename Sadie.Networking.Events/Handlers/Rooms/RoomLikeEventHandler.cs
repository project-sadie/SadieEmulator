using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Networking.Client;
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

        var roomLike = new PlayerRoomLike
        {
            PlayerId = client.Player.Id,
            RoomId = room.Id
        };
        
        client.Player.RoomLikes.Add(roomLike);
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.PlayerRoomLikes.Add(roomLike);
        await dbContext.SaveChangesAsync();
    }
}