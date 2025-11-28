using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Db.Models.Rooms;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

[PacketId(EventHandlerId.RoomCategories)]
public class RoomCategoriesEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var categories = await dbContext
            .Set<RoomCategory>()
            .ToListAsync();
        
        await client.WriteToStreamAsync(new RoomCategoriesWriter
        {
            Categories = mapper.Map<List<RoomCategoryDto>>(categories)
        });
    }
}