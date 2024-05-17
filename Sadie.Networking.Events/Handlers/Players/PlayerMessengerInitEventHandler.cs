using Sadie.Database.Models.Constants;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerIds.PlayerMessengerInit)]
public class PlayerMessengerInitEventHandler(
    ServerPlayerConstants playerConstants)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerMessengerInitWriter
        {
            MaxFriends = playerConstants.MaxFriendships,
            Unknown1 = 1337,
            MaxFriendsHc = playerConstants.MaxFriendships,
            Unknown2 = 0
        });
    }
}