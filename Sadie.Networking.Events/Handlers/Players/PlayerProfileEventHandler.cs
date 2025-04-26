using Sadie.API.Game.Players;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Enums.Game.Players;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerProfile)]
public class PlayerProfileEventHandler(IPlayerRepository playerRepository)
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
        var friendship = client.Player.TryGetFriendshipFor(ProfileId);

        var profileWriter = new PlayerProfileWriter
        {
            Player = profilePlayer,
            Online = profilePlayer.Data.IsOnline,
            FriendshipCount = friendCount,
            FriendshipExists = friendship is { Status: PlayerFriendshipStatus.Accepted },
            FriendshipRequestExists = friendship is { Status: PlayerFriendshipStatus.Pending }
        };
        
        await client.WriteToStreamAsync(profileWriter);
    }
}