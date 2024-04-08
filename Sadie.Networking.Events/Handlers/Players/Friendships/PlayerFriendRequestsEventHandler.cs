using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerFriendRequestsEventHandler() : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerFriendRequestsList;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var pending = client
            .Player
            .Friendships
            .Where(x => x.Status == PlayerFriendshipStatus.Pending)
            .Select(x => x.TargetPlayer)
            .ToList();
        
        await client.WriteToStreamAsync(new PlayerFriendRequestsWriter(pending).GetAllBytes());
    }
}