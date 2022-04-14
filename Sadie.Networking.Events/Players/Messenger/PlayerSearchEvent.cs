using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Players.Messenger;

public class PlayerSearchEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerSearchEvent(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
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

        var strangers = await _playerRepository.GetPlayerDataForSearchAsync(searchQuery, friendships.Select(x => x.Id).ToArray());

        await client.WriteToStreamAsync(new PlayerSearchResultWriter(friendsList, strangers).GetAllBytes());
    }
}