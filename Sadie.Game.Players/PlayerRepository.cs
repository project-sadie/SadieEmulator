using System.Collections.Concurrent;

namespace Sadie.Game.Players;

public class PlayerRepository : IPlayerRepository
{
    private readonly IPlayerDao _playerDao;
    private readonly ConcurrentDictionary<long, IPlayer?> _players;

    public PlayerRepository(IPlayerDao playerDao)
    {
        _playerDao = playerDao;
        _players = new ConcurrentDictionary<long, IPlayer?>();
    }

    public bool TryGetPlayerById(long id, out IPlayer? player)
    {
        return _players.TryGetValue(id, out player);
    }

    public async Task<Tuple<bool, IPlayer?>> TryGetPlayerBySsoAsync(string sso)
    {
        return await _playerDao.TryGetPlayerBySsoTokenAsync(sso);
    }

    public bool TryAddPlayer(IPlayer player) => _players.TryAdd(player.Id, player);

    public async Task MarkPlayerAsOnlineAsync(long id)
    {
        await _playerDao.MarkPlayerAsOnlineAsync(id);
    }

    public async Task MarkPlayerAsOfflineAsync(long id)
    {
        await _playerDao.MarkPlayerAsOfflineAsync(id);
    }

    public async Task ResetSsoTokenForPlayerAsync(long id)
    {
        await _playerDao.ResetSsoTokenForPlayerAsync(id);
    }
}