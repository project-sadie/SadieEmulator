using AutoMapper;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerIds.PlayerProfile)]
public class PlayerProfileEventHandler(
    PlayerProfileEventParser eventParser,
    PlayerRepository playerRepository,
    IMapper mapper)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var profileId = eventParser.ProfileId;
        var profilePlayer = await playerRepository.GetPlayerByIdAsync(profileId);
        
        if (profilePlayer == null)
        {
            return;
        }
        
        var friendCount = profilePlayer.GetAcceptedFriendshipCount();
        var friendship = profilePlayer.TryGetFriendshipFor(profileId);

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