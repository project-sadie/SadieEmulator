using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Messenger;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Shared;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Handlers.Players.Messenger;

public class PlayerSearchEventHandler(PlayerSearchEventParser eventParser, PlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerSearch;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if ((DateTime.Now - client.Player.State.LastPlayerSearch).TotalSeconds < CooldownIntervals.PlayerSearch)
        {
            return;
        }
        
        client.Player.State.LastPlayerSearch = DateTime.Now;

        var searchQuery = eventParser.SearchQuery;

        if (string.IsNullOrEmpty(searchQuery))
        {
            return;
        }

        if (searchQuery.Length > 20)
        {
            searchQuery = searchQuery.Truncate(20);
        }

        var friendships = client.Player!.GetMergedFriendships();
        
        var friendsList = friendships
            .Where(x => x.TargetPlayer.Username.Contains(searchQuery)).
            Select(x => x.TargetPlayer).
            ToList();

        var strangers = await playerRepository.GetPlayersForSearchAsync(searchQuery, friendships.Select(x => x.Id).ToArray());

        await client.WriteToStreamAsync(new PlayerSearchResultWriter(friendsList, strangers).GetAllBytes());
    }
}