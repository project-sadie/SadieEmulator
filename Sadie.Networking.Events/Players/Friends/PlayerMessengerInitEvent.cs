using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friends;

namespace Sadie.Networking.Events.Players.Friends;

public class PlayerMessengerInitEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerMessengerInitWriter(
            int.MaxValue, 
            1337, 
            int.MaxValue, 
            1, 
            1, 
            "Group Chats"
        ).GetAllBytes());
    }
}