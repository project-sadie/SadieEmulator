using Microsoft.EntityFrameworkCore;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Db;
using Sadie.Db.Models.Rooms;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

[PacketId(EventHandlerId.RoomCategories)]
public class RoomCategoriesEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var categories = await dbContext
            .Set<RoomCategory>()
            .ToListAsync();
        
        await client.WriteToStreamAsync(new RoomCategoriesWriter
        {
            Categories = categories
        });
    }
}