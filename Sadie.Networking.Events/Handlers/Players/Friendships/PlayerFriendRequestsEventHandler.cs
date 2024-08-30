using Sadie.Database.Models.Players;
using Sadie.Enums.Game.Players;
using Sadie.Networking.Client;
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

        var requests = new List<IPlayerFriendshipRequestData>()
        {
            {
                new PlayerFriendshipRequestData
                {
                    Username = null,
                    FigureCode = null
                }
            }
        };
        foreach (var request in friendRequests)
        {
            Player data = request.TargetPlayerId == client.Player.Id ? request.OriginPlayer : request.TargetPlayer;
            requests.Add(new PlayerFriendshipRequestData
            {
                Username = null,
                FigureCode = null
            });
        }

        var requestsWriter = new PlayerFriendRequestsWriter
        {
            Requests = requests
        };
        
        await client.WriteToStreamAsync(requestsWriter);
    }
}