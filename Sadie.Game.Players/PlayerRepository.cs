using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
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
        
        await MarkPlayerAsOfflineAsync(player.Data);
        await player!.DisposeAsync();

        return result;
    }

    public async Task MarkPlayerAsOnlineAsync(int id)
    {
        await _playerDataDao.MarkPlayerAsOnlineAsync(id);
    }

    private async Task MarkPlayerAsOfflineAsync(IPlayerData playerData)
    {
        await _playerDataDao.MarkPlayerAsOfflineAsync(playerData);
    }

    public async Task ResetSsoTokenForPlayerAsync(int id)
    {
        await _playerDao.ResetSsoTokenForPlayerAsync(id);
    }

    public int Count()
    {
        return _players.Count;
    }

    public async Task<Tuple<bool, IPlayerData?>> TryGetPlayerData(int playerId)
    {
        return await _playerDataDao.TryGetPlayerData(playerId);
    }

    public async Task<Tuple<bool, IPlayerData?>> TryGetPlayerDataByUsername(string username)
    {
        return await _playerDataDao.TryGetPlayerDataByUsername(username);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var player in _players.Values)
        {
            if (!await TryRemovePlayerAsync(player.Data.Id))
            {
                _logger.LogError($"Failed to properly dispose of player {player.Data.Username}");
            }
        }
    }
}