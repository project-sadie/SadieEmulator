using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

public class RoomCategoriesEventHandler(SadieContext dbContext) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomCategories;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var categories = await dbContext.Set<RoomCategory>().ToListAsync();
        await client.WriteToStreamAsync(new RoomCategoriesWriter(categories).GetAllBytes());
    }
}