using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Players;

public class PlayerMessengerInitEvent(
    IPlayerRepository playerRepository,
    IRoomRepository roomRepository,
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