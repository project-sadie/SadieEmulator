namespace Sadie.Game.Players;

public interface IPlayerRepository
{
    bool TryGetPlayerById(long id, out IPlayer? player);
    Task<Tuple<bool, IPlayer?>> TryGetPlayerBySsoAsync(string sso);
    bool TryAddPlayer(IPlayer? player);
    bool TryRemovePlayer(long playerId);
    Task MarkPlayerAsOnlineAsync(long id);
    Task MarkPlayerAsOfflineAsync(long id);
    Task ResetSsoTokenForPlayerAsync(long id);
    int Count();
}