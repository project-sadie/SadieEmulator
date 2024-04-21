using AutoMapper;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerProfileEventHandler(
    PlayerProfileEventParser eventParser,
    PlayerRepository playerRepository,
    IMapper mapper)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerProfile;

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

        var profileWriter = new PlayerProfileWriter(
            profilePlayer, 
            profilePlayer.Data.IsOnline, 
            friendCount, 
            friendship is { Status: PlayerFriendshipStatus.Accepted }, 
            friendship != null);
        
        await client.WriteToStreamAsync(profileWriter);
    }
}