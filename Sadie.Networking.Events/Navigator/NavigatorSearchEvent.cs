using Sadie.Game.Navigator;
using Sadie.Game.Navigator.Categories;
using Sadie.Game.Navigator.Search;
using Sadie.Game.Navigator.Tabs;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Events.Navigator;

public class NavigatorSearchEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;
    private readonly NavigatorTabRepository _navigatorTabRepository;
    private readonly NavigatorRoomProvider _navigatorRoomProvider;

    public NavigatorSearchEvent(IRoomRepository roomRepository, NavigatorTabRepository navigatorTabRepository, NavigatorRoomProvider navigatorRoomProvider)
    {
        _roomRepository = roomRepository;
        _navigatorTabRepository = navigatorTabRepository;
        _navigatorRoomProvider = navigatorRoomProvider;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var tabName = reader.ReadString();
        var searchQuery = reader.ReadString();

        if (!_navigatorTabRepository.TryGetByCodeName(tabName, out var tab))
        {
            await client.WriteToStreamAsync(new NavigatorSearchResultPagesWriter(tabName, searchQuery, new List<NavigatorCategory>(), _navigatorRoomProvider).GetAllBytes());
            return;
        }

        var categories = tab!.
            Categories.
            OrderBy(x => x.OrderId).
            ToList();

        await client.WriteToStreamAsync(new NavigatorSearchResultPagesWriter(tabName, searchQuery, categories, _navigatorRoomProvider).GetAllBytes());
    }
}