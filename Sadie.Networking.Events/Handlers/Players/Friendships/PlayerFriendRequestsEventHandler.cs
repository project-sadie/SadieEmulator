using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerId.PlayerFriendRequestsList)]
public class PlayerFriendRequestsEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null)
        {
            return;
        }
        
        var friendRequests = client
            .Player
            .IncomingFriendships
            .Where(x => x.Status == PlayerFriendshipStatus.Pending)
            .ToList();

        var requests = (from request in friendRequests 
            let data = request.TargetPlayerId == client.Player.Id ? 
                request.OriginPlayer : 
                request.TargetPlayer select new PlayerFriendshipRequestData { Id = request.Id, Username = data.Username, FigureCode = data.AvatarData.FigureCode })
            .ToList();

        var requestsWriter = new PlayerFriendRequestsWriter
        {
            Requests = requests
        };
        
        await client.WriteToStreamAsync(requestsWriter);
    }
}