using Sadie.Game.Navigator;
using Sadie.Game.Navigator.Categories;
using Sadie.Game.Navigator.Tabs;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Navigator;

public class NavigatorSearchEvent(
    IRoomRepository roomRepository,
    NavigatorTabRepository navigatorTabRepository,
    NavigatorRoomProvider navigatorRoomProvider)
    : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository = roomRepository;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var tabName = reader.ReadString();
        var searchQuery = reader.ReadString();

        if (!navigatorTabRepository.TryGetByCodeName(tabName, out var tab))
        {
            var writer = new NavigatorSearchResultPagesWriter(tabName, searchQuery, new List<NavigatorCategory>(), navigatorRoomProvider);
            
            await client.WriteToStreamAsync(writer.GetAllBytes());
            return;
        }

        var categories = tab!.
            Categories.
            OrderBy(x => x.OrderId).
            ToList();

        var searchResultPagesWriter = new NavigatorSearchResultPagesWriter(
                tabName, 
                searchQuery, 
                categories, 
                navigatorRoomProvider).GetAllBytes();
        
        await client.WriteToStreamAsync(searchResultPagesWriter);
    }
}