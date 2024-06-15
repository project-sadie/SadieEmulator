using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

[PacketId(EventHandlerIds.RoomCategories)]
public class RoomCategoriesEventHandler(SadieContext dbContext) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var categories = await dbContext
            .Set<RoomCategory>()
            .ToListAsync();
        
        await client.WriteToStreamAsync(new RoomCategoriesWriter
        {
            Categories = categories
        });
    }
}