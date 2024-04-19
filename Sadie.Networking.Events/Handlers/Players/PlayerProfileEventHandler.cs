using AutoMapper;
using Sadie.Database.Models.Players;
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

        var player = client.Player;
        var playerId = player.Id;

        var profileId = eventParser.ProfileId;

        Player playerRecord = null;
        var profileOnline = false;
        
        if (profileId == playerId)
        {
            playerRecord = client.Player;
            profileOnline = true;
        }
        
        if (playerRecord == null && playerRepository.TryGetPlayerById(profileId, out var onlinePlayer))
        {
            playerRecord = mapper.Map<Player>(onlinePlayer);
            profileOnline = true;
        }

        if (playerRecord == null)
        {
            return;
        }

        var friendCount = playerRecord.GetAcceptedFriendshipCount();
        var friendship = playerRecord.TryGetFriendshipFor(profileId);

        var profileWriter = new PlayerProfileWriter(
            playerRecord, 
            profileOnline, 
            friendCount, 
            friendship is { Status: PlayerFriendshipStatus.Accepted }, 
            friendship != null);
        
        await client.WriteToStreamAsync(profileWriter);
    }
}