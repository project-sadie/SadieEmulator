using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Players;

public class PlayerProfileEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerFriendshipRepository _friendshipRepository;

    public PlayerProfileEvent(IPlayerRepository playerRepository, IPlayerFriendshipRepository friendshipRepository)
    {
        _playerRepository = playerRepository;
        _friendshipRepository = friendshipRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;
        var playerId = player.Data.Id;
        
        var profileId = reader.ReadInteger();
        var profileOnline = false;

        IPlayerData onlineData = null;
        
        if (profileId == playerId)
        {
            onlineData = client.Player.Data;
            profileOnline = true;
        }
        else if (_playerRepository.TryGetPlayerById(profileId, out var onlinePlayer))
        {
            onlineData = onlinePlayer.Data;
            profileOnline = true;
        }
        else
        {
            var (found, fetchedPlayerData) = await _playerRepository.TryGetPlayerDataAsync(profileId);

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
        var friendshipExists = await _friendshipRepository.DoesFriendshipExist(playerId, profileId);
        var friendshipRequestExists = await _friendshipRepository.DoesRequestExist(playerId, profileId);

        var profileWriter = new PlayerProfileWriter(
                onlineData, 
                profileOnline, 
                friendCount, 
                friendshipExists, 
                friendshipRequestExists).GetAllBytes();
        
        await client.WriteToStreamAsync(profileWriter);
    }
}