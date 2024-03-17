using Sadie.Game.Navigator;
using Sadie.Game.Navigator.Categories;
using Sadie.Game.Navigator.Tabs;
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
                playerId, 
                tabName, 
                searchQuery, 
                new List<NavigatorCategory>(), 
                navigatorRoomProvider);
            
            await client.WriteToStreamAsync(writer.GetAllBytes());
            return;
        }

        var categories = tab!.
            Categories.
            OrderBy(x => x.OrderId).
            ToList();

        var searchResultPagesWriter = new NavigatorSearchResultPagesWriter(
            playerId,
            tabName, 
            searchQuery, 
            categories, 
            navigatorRoomProvider).GetAllBytes();
        
        await client.WriteToStreamAsync(searchResultPagesWriter);
    }
}