using Sadie.Game.Navigator;
using Sadie.Game.Navigator.Categories;
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
        
        var searchResultPagesWriter = new NavigatorSearchResultPagesWriter(
            tabName, 
            searchQuery, 
            categoryRoomMap).GetAllBytes();
        
        await client.WriteToStreamAsync(searchResultPagesWriter);
    }
}