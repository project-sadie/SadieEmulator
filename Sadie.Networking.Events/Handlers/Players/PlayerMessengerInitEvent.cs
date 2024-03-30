using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerMessengerInitEvent(
    PlayerConstants playerConstants)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerMessengerInitWriter(
            playerConstants.MaxFriendships,
            1337, 
            playerConstants.MaxFriendships,
            0
        ).GetAllBytes());
    }
}