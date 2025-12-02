using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Core.Shared.Constants;
using Sadie.Core.Shared.Extensions;
using Sadie.Networking.Packets.Writers.Players.Messenger;

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
            .Player.OutgoingFriendships
            .Select(x => x.TargetPlayer!);
        
        var incomingFriends = client
            .Player!
            .Player.IncomingFriendships
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