using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Packets.Writers.Players;
using PlayerFriendshipStatus = Sadie.Core.Enums.Game.Players.PlayerFriendshipStatus;

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
        
        var incomingAccepted = profilePlayer.IncomingFriendships.Count(x => x.Status == PlayerFriendshipStatus.Accepted);
        var outgoingAccepted = profilePlayer.OutgoingFriendships.Count(x => x.Status == PlayerFriendshipStatus.Accepted);
        var acceptedFriendCount = incomingAccepted + outgoingAccepted;
        
        var friendship = client.Player.TryGetFriendshipFor(ProfileId);

        var profileWriter = new PlayerProfileWriter
        {
            Player = profilePlayer,
            Online = profilePlayer.Data.IsOnline,
            FriendshipCount = acceptedFriendCount,
            FriendshipExists = friendship is { Status: PlayerFriendshipStatus.Accepted },
            FriendshipRequestExists = friendship is { Status: PlayerFriendshipStatus.Pending }
        };
        
        await client.WriteToStreamAsync(profileWriter);
    }
}