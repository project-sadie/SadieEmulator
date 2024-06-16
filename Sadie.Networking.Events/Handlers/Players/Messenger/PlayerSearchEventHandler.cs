using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Shared;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Handlers.Players.Messenger;

[PacketId(EventHandlerIds.PlayerSearch)]
public class PlayerSearchEventHandler(PlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public string? SearchQuery { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if ((DateTime.Now - client.Player.State.LastPlayerSearch).TotalSeconds < CooldownIntervals.PlayerSearch)
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