using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Friendships;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerIds.PlayerFriendListUpdate)]
public class PlayerFriendListUpdateEventHandler(
    PlayerRepository playerRepository)
    : INetworkPacketEventHandler
{
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
            
            await client.WriteToStreamAsync(new PlayerFriendsListWriter
            {
                Pages = pages,
                Index = i,
                Friends = batch,
                PlayerRepository = playerRepository,
                Relationships = player.Relationships
            });
        }
    }
}