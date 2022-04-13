using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Sadie.Shared.Networking;

namespace Sadie.Game.Players;

public class PlayerRepository : IPlayerRepository
{
    private readonly ILogger<PlayerRepository> _logger;
    private readonly IPlayerDao _playerDao;
    private readonly ConcurrentDictionary<int, IPlayer> _players;

    public PlayerRepository(ILogger<PlayerRepository> logger, IPlayerDao playerDao)
    {
        _logger = logger;
        _playerDao = playerDao;
        _players = new ConcurrentDictionary<int, IPlayer>();
    }

    public bool TryGetPlayerById(int id, out IPlayer? player)
    {
        return _players.TryGetValue(id, out player);
    }

    public bool TryGetPlayerByUsername(string username, out IPlayer? player)
    {
        player = _players.Values.FirstOrDefault(x => x.Username == username);
        return player != default;
    }

    public async Task<Tuple<bool, IPlayer?>> TryGetPlayerBySsoAsync(INetworkObject networkObject, string sso)
    {
        return await _playerDao.TryGetPlayerBySsoTokenAsync(networkObject, sso);
    }

    public bool TryAddPlayer(IPlayer player) => _players.TryAdd(player.Id, player);

    public async Task<bool> TryRemovePlayerAsync(int playerId)
    {
        var result = _players.TryRemove(playerId, out var player);

        if (player == null)
        {
            return result;
        }
        
        await MarkPlayerAsOfflineAsync(player);
        await player!.DisposeAsync();

        return result;
    }

    public async Task MarkPlayerAsOnlineAsync(int id)
    {
        await _playerDao.MarkPlayerAsOnlineAsync(id);
    }

    private async Task MarkPlayerAsOfflineAsync(IPlayer player)
    {
        await _playerDao.MarkPlayerAsOfflineAsync(player);
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
        return await _playerDao.TryGetPlayerData(playerId);
    }

    public async Task<Tuple<bool, IPlayerData?>> TryGetPlayerDataByUsername(string username)
    {
        return await _playerDao.TryGetPlayerDataByUsername(username);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var player in _players.Values)
        {
            if (!await TryRemovePlayerAsync(player.Id))
            {
                _logger.LogError($"Failed to properly dispose of player {player.Username}");
            }
        }
    }
}