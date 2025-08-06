using Microsoft.EntityFrameworkCore;
using Sadie.Db;
using Sadie.Networking.Client;
using Sadie.Networking.Writers.Players.Rooms;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Players.Club;

[PacketId(EventHandlerId.PlayerSetHomeRoom)]
public class PlayerSetHomeRoomEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory) : INetworkPacketEventHandler
{
    public int RoomId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player?.NetworkObject == null ||
            client.Player.Data.HomeRoomId == RoomId)
        {
            return;
        }

        await client.Player.NetworkObject.WriteToStreamAsync(new PlayerHomeRoomWriter
        {
            HomeRoom = RoomId,
            RoomIdToEnter = 0
        });
        
        client.Player.Data.HomeRoomId = RoomId;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        dbContext
            .Entry(client.Player.Data)
            .Property(x => x.HomeRoomId)
            .IsModified = true;
        
        await dbContext.SaveChangesAsync();
    }
}