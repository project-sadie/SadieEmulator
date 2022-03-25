namespace Sadie.Game.Players;

public interface IPlayerDao
{
    Task<Tuple<bool, IPlayer?>> TryGetPlayerBySsoTokenAsync(string sso);
    Task MarkPlayerAsOnlineAsync(long id);
    Task MarkPlayerAsOfflineAsync(IPlayer player);
    Task ResetSsoTokenForPlayerAsync(long id);
}