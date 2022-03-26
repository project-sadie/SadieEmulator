using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Messenger;

namespace Sadie.Networking.Events.Players.Messenger;

public class PlayerFriendRequestsListEvent : INetworkPacketEvent
{
    private readonly IPlayerFriendshipRepository _friendshipRepository;

    public PlayerFriendRequestsListEvent(IPlayerFriendshipRepository friendshipRepository)
    {
        _friendshipRepository = friendshipRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var friends = await _friendshipRepository.GetFriendshipRequestsAsync(client.Player.Id);
        await client.WriteToStreamAsync(new PlayerFriendRequestsWriter(friends).GetAllBytes());
    }
}