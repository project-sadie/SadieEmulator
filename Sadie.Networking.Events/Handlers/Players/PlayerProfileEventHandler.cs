using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerIds.PlayerProfile)]
public class PlayerProfileEventHandler(PlayerRepository playerRepository)
    : INetworkPacketEventHandler
{
    public int ProfileId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var profilePlayer = await playerRepository.GetPlayerByIdAsync(ProfileId);
        
        if (profilePlayer == null)
        {
            return;
        }
        
        var friendCount = profilePlayer.GetAcceptedFriendshipCount();
        var friendship = profilePlayer.TryGetFriendshipFor(ProfileId);

        var profileWriter = new PlayerProfileWriter
        {
            Player = profilePlayer,
            Online = profilePlayer.Data.IsOnline,
            FriendshipCount = friendCount,
            FriendshipExists = friendship is { Status: PlayerFriendshipStatus.Accepted },
            FriendshipRequestExists = friendship!= null
        };
        
        await client.WriteToStreamAsync(profileWriter);
    }
}