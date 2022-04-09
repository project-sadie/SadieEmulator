namespace Sadie.Game.Players;

public interface IPlayerRepository : IAsyncDisposable
{
    bool TryGetPlayerById(long id, out IPlayer? player);
    bool TryGetPlayerByUsername(string username, out IPlayer? player);
    Task<Tuple<bool, IPlayer?>> TryGetPlayerBySsoAsync(string sso);
    bool TryAddPlayer(IPlayer player);
    Task<bool> TryRemovePlayerAsync(long playerId);
    Task MarkPlayerAsOnlineAsync(long id);
    Task MarkPlayerAsOfflineAsync(IPlayer player);
    Task ResetSsoTokenForPlayerAsync(long id);
    int Count();
    Task<Tuple<bool, IPlayerData?>> TryGetPlayerData(long playerId);
    Task<Tuple<bool, IPlayerData?>> TryGetPlayerData(string username);
}