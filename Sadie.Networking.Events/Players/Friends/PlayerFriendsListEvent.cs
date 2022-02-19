using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Players.Friends;

namespace Sadie.Networking.Packets.Client.Players.Friends;

public class PlayerFriendsListEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerFriendsListWriter(0, 0, 0).GetAllBytes());
    }
}