using Sadie.Game.Navigator;
using Sadie.Game.Navigator.Categories;sea
using Sadie.Game.Navigator.Tabs;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Navigator;

public class NavigatorSearchEvent(
    NavigatorTabRepository navigatorTabRepository,
    NavigatorRoomProvider navigatorRoomProvider)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player == null)
        {
            return;
        }

        var playerId = client.Player.Data.Id;
        var tabName = reader.ReadString();
        var searchQuery = reader.ReadString();

        if (!navigatorTabRepository.TryGetByCodeName(tabName, out var tab))
        {
            var writer = new NavigatorSearchResultPagesWriter(
                tabName, 
                searchQuery, 
                new Dictionary<NavigatorCategory, List<IRoom>>());
            
            await client.WriteToStreamAsync(writer.GetAllBytes());
            return;
        }

        var categories = tab!.
            Categories.
            OrderBy(x => x.OrderId).
            ToList();

        var categoryRoomMap = new Dictionary<NavigatorCategory, List<IRoom>>();

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

    private static Dictionary<NavigatorCategory, List<IRoom>> ApplyFilter(
        string searchQuery, 
        Dictionary<NavigatorCategory, List<IRoom>> categoryRoomMap)
    {
        if (searchQuery.Contains(':'))
        {
            var searchQueryParts = searchQuery.Split(":");
            var filterName = searchQueryParts[0];
            var filterValue = searchQuery.Substring(filterName.Length + 1);

            if (string.IsNullOrEmpty(filterValue))
            {
                return categoryRoomMap;
            }
            
            switch (filterName)
            {
                case "roomname":
                    return categoryRoomMap.Where(x => x.Value.Any(r => r.Name.Contains(filterValue))).ToDictionary();
                case "owner":
                    return categoryRoomMap.Where(x => x.Value.Any(r => r.OwnerName.Contains(filterValue))).ToDictionary();
                case "tag":
                    return categoryRoomMap.Where(x => x.Value.Any(r => r.Tags.Any(t => t.Contains(filterValue)))).ToDictionary();
            }
        }
        else if (!string.IsNullOrEmpty(searchQuery))
        {
            // TODO: Filter on anything
        }

        return categoryRoomMap;
    }
}