using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Navigator;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Navigator;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Db.Models.Navigator;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

[PacketId(EventHandlerId.NavigatorSearch)]
public class NavigatorSearchEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    INavigatorRoomProvider navigatorRoomProvider,
    IRoomRepository roomRepository,
    IMapper mapper)
    : INetworkPacketEventHandler
{
    public string? TabName { get; set; }
    public string? SearchQuery { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null)
        {
            return;
        }
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var tab = await dbContext.Set<NavigatorTab>()
            .Include(x => x.Categories)
            .FirstOrDefaultAsync(x => x.Name == TabName);

        if (tab == null)
        {
            return;
        }

        var dbCategories = tab.
            Categories.
            OrderBy(x => x.OrderId).
            ToList();
        
        var categories = mapper.Map<List<NavigatorCategoryDto>>(dbCategories);

        var categoryRoomMap = new Dictionary<NavigatorCategoryDto, List<RoomDto>>();

        if (!string.IsNullOrEmpty(SearchQuery))
        {
            categoryRoomMap[new NavigatorCategoryDto
            {
                Name = "Search Results",
                CodeName = "",
                OrderId = 0,
                TabId = 1
            }] = await navigatorRoomProvider.GetRoomsForSearchQueryAsync(SearchQuery);
        }
        else
        {
            foreach (var category in categories)
            {
                categoryRoomMap.Add(category, await navigatorRoomProvider.GetRoomsForCategoryNameAsync(client.Player, category.CodeName));
            }
        }
        
        var searchResultPagesWriter = new NavigatorSearchResultPagesWriter
        {
            TabName = TabName,
            SearchQuery = SearchQuery,
            CategoryRoomMap = categoryRoomMap,
            RoomRepository = roomRepository
        };
        
        await client.WriteToStreamAsync(searchResultPagesWriter);
    }
}