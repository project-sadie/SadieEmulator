using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Shared;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Players.Messenger;

public class PlayerSearchEvent(IPlayerRepository playerRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if ((DateTime.Now - client.Player.State.LastPlayerSearch).TotalSeconds < CooldownSeconds.PlayerSearch)
        {
            return;
        }
        
        client.Player.State.LastPlayerSearch = DateTime.Now;
        
        var searchQuery = reader.ReadString();

        if (string.IsNullOrEmpty(searchQuery))
        {
            return;
        }

        if (searchQuery.Length > 20)
        {
            searchQuery = searchQuery.Truncate(20);
        }

        var friendships = client.Player!.Data.FriendshipComponent.Friendships;
        
        var friendsList = friendships
            .Where(x => x.TargetData.Username.Contains(searchQuery)).
            Select(x => x.TargetData).
            ToList();

        var strangers = await playerRepository.GetPlayerDataForSearchAsync(searchQuery, friendships.Select(x => x.Id).ToArray());

        await client.WriteToStreamAsync(new PlayerSearchResultWriter(friendsList, strangers).GetAllBytes());
    }
}