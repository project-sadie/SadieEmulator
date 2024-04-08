using Sadie.Database.Models.Navigator;
using Sadie.Game.Navigator;
using Sadie.Game.Navigator.Tabs;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Navigator;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

public class NavigatorSearchEventHandler(
    NavigatorSearchEventParser eventParser,
    NavigatorTabRepository navigatorTabRepository,
    NavigatorRoomProvider navigatorRoomProvider)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.NavigatorSearch;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if (client.Player == null)
        {
            return;
        }

        var playerId = client.Player.Id;
        var tabName = eventParser.TabName;
        var searchQuery = eventParser.SearchQuery;

        if (!navigatorTabRepository.TryGetByCodeName(tabName, out var tab))
        {
            var writer = new NavigatorSearchResultPagesWriter(
                tabName, 
                searchQuery, 
                new Dictionary<NavigatorCategory, List<RoomLogic>>());
            
            await client.WriteToStreamAsync(writer.GetAllBytes());
            return;
        }

        var categories = tab!.
            Categories.
            OrderBy(x => x.OrderId).
            ToList();

        var categoryRoomMap = new Dictionary<NavigatorCategory, List<RoomLogic>>();

        foreach (var category in categories)
        {
            categoryRoomMap.Add(category, await navigatorRoomProvider.GetRoomsForCategoryNameAsync(playerId, category.CodeName));
        }

        categoryRoomMap = ApplyFilter(searchQuery, categoryRoomMap);
        
        var searchResultPagesWriter = new NavigatorSearchResultPagesWriter(
            tabName, 
            searchQuery, 
            categoryRoomMap).GetAllBytes();
        
        await client.WriteToStreamAsync(searchResultPagesWriter);
    }

    private static Dictionary<NavigatorCategory, List<RoomLogic>> ApplyFilter(
        string searchQuery, 
        Dictionary<NavigatorCategory, List<RoomLogic>> categoryRoomMap)
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