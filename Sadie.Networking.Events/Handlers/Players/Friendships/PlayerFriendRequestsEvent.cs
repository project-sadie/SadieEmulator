using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Messenger;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerFriendRequestsEvent(IPlayerFriendshipRepository friendshipRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var pendingFriends = await friendshipRepository.GetIncomingFriendRequestsForPlayerAsync(client.Player!.Data.Id);
        await client.WriteToStreamAsync(new PlayerFriendRequestsWriter(pendingFriends.Select(x => x.TargetData).ToList()).GetAllBytes());
    }
}