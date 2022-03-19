using Sadie.Game.Navigator;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Navigator;

public class NavigatorSearchEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public NavigatorSearchEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var tabName = reader.ReadString();
        var searchQuery = reader.ReadString();
        var results = new List<NavigatorSearchResult>();
        var rooms = _roomRepository.GetAll();
        
        results.Add(new NavigatorSearchResult(0, tabName, searchQuery, rooms, NavigatorSearchAction.None, false));
        await client.WriteToStreamAsync(new NavigatorSearchResultPagesWriter(tabName, searchQuery, results).GetAllBytes());
    }
}