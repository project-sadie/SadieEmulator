using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friends;

namespace Sadie.Networking.Events.Players.Friends;

public class PlayerFriendsListEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerFriendsListWriter(0, 0, 0).GetAllBytes());
    }
}