using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Packets;
using Sadie.Shared.Networking;

namespace Sadie.Game.Players;

public class PlayerRepository : IPlayerRepository
{
    private readonly ILogger<PlayerRepository> _logger;
    private readonly IPlayerDao _playerDao;
    private readonly IPlayerDataDao _playerDataDao;
    private readonly ConcurrentDictionary<int, IPlayer> _players;

    public PlayerRepository(ILogger<PlayerRepository> logger, IPlayerDao playerDao, IPlayerDataDao playerDataDao)
    {
        _logger = logger;
        _playerDao = playerDao;
        _playerDataDao = playerDataDao;
        _players = new ConcurrentDictionary<int, IPlayer>();
    }

    public bool TryGetPlayerById(int id, out IPlayer? player)
    {
        return _players.TryGetValue(id, out player);
    }

    public bool TryGetPlayerByUsername(string username, out IPlayer? player)
    {
        player = _players.Values.FirstOrDefault(x => x.Data.Username == username);
        return player != default;
    }

    public async Task<Tuple<bool, IPlayer?>> TryGetPlayerBySsoAsync(INetworkObject networkObject, string sso)
    {
        return await _playerDao.TryGetPlayerBySsoTokenAsync(networkObject, sso);
    }

    public bool TryAddPlayer(IPlayer player) => _players.TryAdd(player.Data.Id, player);

    public async Task<bool> TryRemovePlayerAsync(int playerId)
    {
        var result = _players.TryRemove(playerId, out var player);

        if (player == null)
        {
            return result;
        }

        await MarkPlayerAsOfflineAsync(player.Data, player.State);
        await UpdateMessengerStatusForFriends(player.Data.Id, player.Data.FriendshipComponent.Friendships, false, false);
        await player!.DisposeAsync();

        return result;
    }

    public async Task UpdateMessengerStatusForFriends(int playerId, IEnumerable<PlayerFriendship> friendships, bool isOnline, bool inRoom)
    {
        foreach (var friendId in friendships.Select(x => x.TargetData.Id).Distinct())
        {
            if (!TryGetPlayerById(friendId, out var friend) || friend == null)
            {
                continue;
            }
         
            var friendship = friend.Data.FriendshipComponent.Friendships.FirstOrDefault(x => x.TargetData.Id == playerId);

            if (friendship != null)
            {
                await friend.NetworkObject.WriteToStreamAsync(new PlayerUpdateFriendWriter(0, 1, 0, friendship, isOnline, inRoom, 0, "", "",false, false, false).GetAllBytes());
            }
        }
    }
    
    public async Task MarkPlayerAsOnlineAsync(int id)
    {
        await _playerDataDao.MarkPlayerAsOnlineAsync(id);
    }

    private async Task MarkPlayerAsOfflineAsync(IPlayerData data, IPlayerState state)
    {
        await _playerDataDao.MarkPlayerAsOfflineAsync(data, state);
    }

    public async Task ResetSsoTokenForPlayerAsync(int id)
    {
        await _playerDao.ResetSsoTokenForPlayerAsync(id);
    }

    public int Count()
    {
        return _players.Count;
    }

    public async Task<Tuple<bool, IPlayerData?>> TryGetPlayerDataAsync(int playerId)
    {
        return await _playerDataDao.TryGetPlayerData(playerId);
    }

    public async Task<Tuple<bool, IPlayerData?>> TryGetPlayerDataByUsernameAsync(string username)
    {
        return await _playerDataDao.TryGetPlayerDataByUsername(username);
    }

    public async Task<List<IPlayerData>> GetPlayerDataForSearchAsync(string searchQuery, int[] excludeIds)
    {
        return await _playerDataDao.GetPlayerDataForSearch(searchQuery, excludeIds);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var player in _players.Values)
        {
            if (!await TryRemovePlayerAsync(player.Data.Id))
            {
                _logger.LogError("Failed to dispose of player");
            }
        }
    }
}