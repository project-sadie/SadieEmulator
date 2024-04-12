using Sadie.Database.Models.Constants;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerMessengerInitEventHandler(
    ServerPlayerConstants playerConstants)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerMessengerInit;

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