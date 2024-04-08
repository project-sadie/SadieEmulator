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
        var playerId = player.Data.Id;

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
        
        var friendCount = playerRecord.Friendships.Count;

        var friendship = playerRecord
            .Friendships
            .FirstOrDefault(x => x.OriginPlayerId == playerRecord.Id || x.TargetPlayerId == playerRecord.Id);

        var friendshipRequestExists = friendship is { Status: PlayerFriendshipStatus.Pending };

        var profileWriter = new PlayerProfileWriter(
            playerRecord, 
            profileOnline, 
            friendCount, 
            friendship != null, 
            friendshipRequestExists).GetAllBytes();
        
        await client.WriteToStreamAsync(profileWriter);
    }
}