using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Players;

public class PlayerProfileEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerDao _playerDao;
    private readonly IPlayerFriendshipRepository _friendshipRepository;

    public PlayerProfileEvent(IPlayerRepository playerRepository, IPlayerDao playerDao, IPlayerFriendshipRepository friendshipRepository)
    {
        _playerRepository = playerRepository;
        _playerDao = playerDao;
        _friendshipRepository = friendshipRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerId = reader.ReadInt();
        var isThemselves = reader.ReadBool();
        var playerOnline = isThemselves;

        IPlayerData playerData = null;
        
        if (isThemselves)
        {
            playerData = client.Player;
        }
        else if (_playerRepository.TryGetPlayerById(playerId, out var player))
        {
            playerData = player;
            playerOnline = true;
        }
        else
        {
            var (found, fetchedPlayerData) = await _playerDao.TryGetPlayerData(playerId);

            if (found)
            {
                playerData = fetchedPlayerData;
            }
        }

        if (playerData == null)
        {
            return;
        }

        var playerFriends = await _friendshipRepository.GetActiveFriendsCountAsync(playerId);
        var friendshipExists = await _friendshipRepository.DoesFriendshipExist(client.Player.Id, playerId, PlayerFriendshipStatus.Accepted);
        var friendshipRequestExists = await _friendshipRepository.DoesFriendshipExist(client.Player.Id, playerId, PlayerFriendshipStatus.Pending);
        
        await client.WriteToStreamAsync(new PlayerProfileWriter(playerData, playerOnline, playerFriends, friendshipExists, friendshipRequestExists).GetAllBytes());
    }
}