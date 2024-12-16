using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

[PacketId(EventHandlerId.RoomCategories)]
public class RoomCategoriesEventHandler(IDbContextFactory<SadieContext> dbContextFactory) : INetworkPacketEventHandler
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