using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerFriendsListEventHandler(
    PlayerRepository playerRepository)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerFriendsList;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player!;

        var friends = player
            .GetMergedFriendships()
            .Where(x => x.Status == PlayerFriendshipStatus.Accepted)
            .ToList();
        
        var pages = friends.Count / 500 + 1;
        
        for (var i = 0; i < pages; i++)
        {
            var batch = friends.Skip(i * 500).
                Take(500).
                ToList();
            
            await client.WriteToStreamAsync(new PlayerFriendsListWriter(
                pages, 
                i, 
                batch, 
                playerRepository,
                player.Relationships).GetAllBytes());
        }
    }
}