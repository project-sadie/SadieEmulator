using Sadie.Database.Models.Constants;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerMessengerInit)]
public class PlayerMessengerInitEventHandler(
    ServerPlayerConstants playerConstants)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new PlayerMessengerInitWriter
        {
            UserFriendLimit = playerConstants.MaxFriendships,
            NormalFriendLimit = playerConstants.MaxFriendships,
            ExtendedFriendLimit = playerConstants.MaxFriendships,
            FriendshipCategories = []
        });
    }
}