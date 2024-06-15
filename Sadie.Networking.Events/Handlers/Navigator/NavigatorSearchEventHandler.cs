using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Navigator;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Navigator;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

[PacketId(EventHandlerIds.NavigatorSearch)]
public class NavigatorSearchEventHandler(
    SadieContext dbContext,
    NavigatorRoomProvider navigatorRoomProvider,
    RoomRepository roomRepository)
    : INetworkPacketEventHandler
{
    [PacketData] public string? TabName { get; set; }
    [PacketData] public string? SearchQuery { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null)
        {
            return;
        }
        
        var tab = await dbContext.Set<NavigatorTab>()
            .Include(x => x.Categories)
            .FirstOrDefaultAsync(x => x.Name == TabName);

        if (tab == null)
        {
            return;
        }

        var categories = tab!.
            Categories.
            OrderBy(x => x.OrderId).
            ToList();

        var categoryRoomMap = new Dictionary<NavigatorCategory, List<Room>>();

        foreach (var category in categories)
        {
            categoryRoomMap.Add(category, await navigatorRoomProvider.GetRoomsForCategoryNameAsync(client.Player, category.CodeName));
        }

        categoryRoomMap = ApplyFilter(SearchQuery, categoryRoomMap);
        
        var searchResultPagesWriter = new NavigatorSearchResultPagesWriter
        {
            TabName = TabName,
            SearchQuery = SearchQuery,
            CategoryRoomMap = categoryRoomMap,
            RoomRepository = roomRepository
        };
        
        await client.WriteToStreamAsync(searchResultPagesWriter);
    }

    private static Dictionary<NavigatorCategory, List<Room>> ApplyFilter(
        string searchQuery, 
        Dictionary<NavigatorCategory, List<Room>> categoryRoomMap)
    {
        if (searchQuery.Contains(':'))
        {
            var searchQueryParts = searchQuery.Split(":");
            var filterName = searchQueryParts[0];
            var filterValue = searchQuery[(filterName.Length + 1)..];

            if (string.IsNullOrEmpty(filterValue))
            {
                return categoryRoomMap;
            }
            
            switch (filterName)
            {
                case "roomname":
                    return categoryRoomMap
                        .Where(x => x.Value.Any(r => r.Name.Contains(filterValue, StringComparison.OrdinalIgnoreCase)))
                        .ToDictionary();
                case "owner":
                    return categoryRoomMap
                        .Where(x => x.Value.Any(r => r.Owner.Username.Contains(filterValue, StringComparison.OrdinalIgnoreCase)))
                        .ToDictionary();
                case "tag":
                    return categoryRoomMap
                        .Where(x => x.Value.Any(r => r.Tags.Any(t => t.Name.Contains(filterValue, StringComparison.OrdinalIgnoreCase))))
                        .ToDictionary();
            }
        }
        else if (!string.IsNullOrEmpty(searchQuery))
        {
            // TODO: Filter on anything
        }

        return categoryRoomMap;
    }
}