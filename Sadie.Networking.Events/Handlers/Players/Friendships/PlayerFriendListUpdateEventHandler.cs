using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerFriendListUpdateEventHandler(
    IPlayerRepository playerRepository)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerFriendListUpdate;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player!;
        var friends = player.Data.FriendshipComponent.Friendships;
        var pages = friends.Count / 500 + 1;
        
        for (var i = 0; i < pages; i++)
        {
            var batch = friends.Skip(i * 500).
                Take(500).
                ToList();
            
            await client.WriteToStreamAsync(new PlayerFriendsListWriter(
                pages, i, batch, 
                playerRepository, 
                player.Data.Relationships).GetAllBytes());
        }
    }
}