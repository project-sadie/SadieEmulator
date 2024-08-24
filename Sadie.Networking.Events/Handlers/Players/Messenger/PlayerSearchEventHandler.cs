using Sadie.API.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Shared.Constants;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Handlers.Players.Messenger;

[PacketId(EventHandlerId.PlayerSearch)]
public class PlayerSearchEventHandler(IPlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public string? SearchQuery { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if ((DateTime.Now - client.Player.State.LastPlayerSearch).TotalMilliseconds < CooldownIntervals.PlayerSearch)
        {
            return;
        }
        
        client.Player.State.LastPlayerSearch = DateTime.Now;

        if (string.IsNullOrEmpty(SearchQuery))
        {
            return;
        }

        SearchQuery = SearchQuery.Truncate(20);

        var outgoingFriends = client
            .Player!
            .OutgoingFriendships
            .Select(x => x.TargetPlayer!);
        
        var incomingFriends = client
            .Player!
            .IncomingFriendships
            .Select(x => x.OriginPlayer!);

        var friendsList = outgoingFriends
            .Concat(incomingFriends)
            .DistinctBy(x => x.Id)
            .Where(x => x.Username.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var strangers = await playerRepository
            .GetPlayersForSearchAsync(SearchQuery, friendsList.Select(x => x.Id).ToArray());

        await client.WriteToStreamAsync(new PlayerSearchResultWriter
        {
            Friends = friendsList,
            Strangers = strangers
        });
    }
}