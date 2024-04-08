using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Messenger;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerFriendRequestsEventHandler(IPlayerFriendshipRepository friendshipRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerFriendRequestsList;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var pendingFriends = await friendshipRepository.GetIncomingFriendRequestsForPlayerAsync(client.Player!.Data.Id);
        await client.WriteToStreamAsync(new PlayerFriendRequestsWriter(pendingFriends.Select(x => x.TargetData).ToList()).GetAllBytes());
    }
}