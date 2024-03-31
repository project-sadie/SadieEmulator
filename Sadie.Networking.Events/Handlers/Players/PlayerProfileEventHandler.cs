using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerProfileEventHandler(
    PlayerProfileEventParser eventParser,
    IPlayerRepository playerRepository, 
    IPlayerFriendshipRepository friendshipRepository)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerProfile;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var player = client.Player;
        var playerId = player.Data.Id;

        var profileId = eventParser.ProfileId;
        var profileOnline = false;

        IPlayerData onlineData = null;
        
        if (profileId == playerId)
        {
            onlineData = client.Player.Data;
            profileOnline = true;
        }
        else if (playerRepository.TryGetPlayerById(profileId, out var onlinePlayer))
        {
            onlineData = onlinePlayer.Data;
            profileOnline = true;
        }
        else
        {
            var (found, fetchedPlayerData) = await playerRepository.TryGetPlayerDataAsync(profileId);

            if (found)
            {
                onlineData = fetchedPlayerData;
            }
        }

        if (onlineData == null)
        {
            return;
        }

        var friendCount = onlineData.FriendshipComponent.Friendships.Count;
        var friendshipExists = await friendshipRepository.DoesFriendshipExist(playerId, profileId);
        var friendshipRequestExists = await friendshipRepository.DoesRequestExist(playerId, profileId);

        var profileWriter = new PlayerProfileWriter(
                onlineData, 
                profileOnline, 
                friendCount, 
                friendshipExists, 
                friendshipRequestExists).GetAllBytes();
        
        await client.WriteToStreamAsync(profileWriter);
    }
}