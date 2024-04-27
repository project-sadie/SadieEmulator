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
        if (client.Player == null)
        {
            return;
        }
        
        var pending = client
            .Player
            .OutgoingFriendships
            .Where(x => x.Status == PlayerFriendshipStatus.Pending)
            .Select(x => x.TargetPlayer)
            .ToList();

        var requestsWriter = new PlayerFriendRequestsWriter
        {
            Requests = pending
        };
        
        await client.WriteToStreamAsync(requestsWriter);
    }
}