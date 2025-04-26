using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Enums.Game.Players;
using Sadie.Networking.Events.Dtos;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Messenger;
using IPlayerFriendshipRequestData = Sadie.API.Game.Players.Friendships.IPlayerFriendshipRequestData;

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

        var requests = new List<IPlayerFriendshipRequestData>();
        
        foreach (var data in friendRequests.Select(request => request.TargetPlayerId == client.Player.Id ? 
                     request.OriginPlayer : 
                     request.TargetPlayer))
        {
            if (data?.AvatarData == null)
            {
                continue;
            }
            
            requests.Add(new PlayerFriendshipRequestData
            {
                Username = data.Username,
                FigureCode = data.AvatarData.FigureCode
            });
        }

        var requestsWriter = new PlayerFriendRequestsWriter
        {
            TotalRequests = requests.Count,
            Requests = requests
        };
        
        await client.WriteToStreamAsync(requestsWriter);
    }
}