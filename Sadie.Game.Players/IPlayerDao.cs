using Sadie.Shared.Networking;

namespace Sadie.Game.Players;

public interface IPlayerDao
{
    Task<Tuple<bool, IPlayer?>> TryGetPlayerBySsoTokenAsync(INetworkObject networkObject, string sso);
    Task MarkPlayerAsOnlineAsync(long id);
    Task MarkPlayerAsOfflineAsync(IPlayer player);
    Task ResetSsoTokenForPlayerAsync(long id);
    Task<Tuple<bool, IPlayerData?>> TryGetPlayerData(long playerId);
    Task<Tuple<bool, IPlayerData?>> TryGetPlayerDataByUsername(string username);
}