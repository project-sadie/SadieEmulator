using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms;

public class RoomLikeEventHandler(RoomRepository roomRepository,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomLike;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
        {
            return;
        }

        if (room.OwnerId == client.Player.Id || client.Player.RoomLikes.FirstOrDefault(x => x.RoomId == room.Id) != null)
        {
            return;
        }
        
        client.Player.RoomLikes.Add(new PlayerRoomLike()
        {
            PlayerId = client.Player.Id,
            RoomId = room.Id
        });
        await dbContext.SaveChangesAsync();
    }
}