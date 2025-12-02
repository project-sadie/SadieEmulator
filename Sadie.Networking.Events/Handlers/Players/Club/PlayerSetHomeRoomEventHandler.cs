using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Networking.Packets.Writers.Players.Rooms;

namespace Sadie.Networking.Events.Handlers.Players.Club;

[PacketId(EventHandlerId.PlayerSetHomeRoom)]
public class PlayerSetHomeRoomEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory) : INetworkPacketEventHandler
{
    public int RoomId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player?.NetworkObject == null ||
            client.Player.Player.Data.HomeRoomId == RoomId)
        {
            return;
        }

        await client.Player.NetworkObject.WriteToStreamAsync(new PlayerHomeRoomWriter
        {
            HomeRoom = RoomId,
            RoomIdToEnter = 0
        });
        
        client.Player.Player.Data.HomeRoomId = RoomId;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        dbContext
            .Entry(client.Player.Player.Data)
            .Property(x => x.HomeRoomId)
            .IsModified = true;
        
        await dbContext.SaveChangesAsync();
    }
}