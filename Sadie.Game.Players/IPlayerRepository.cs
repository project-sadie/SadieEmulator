using Sadie.Shared.Networking;

namespace Sadie.Game.Players;

public interface IPlayerRepository : IAsyncDisposable
{
    bool TryGetPlayerById(int id, out IPlayer? player);
    bool TryGetPlayerByUsername(string username, out IPlayer? player);
    Task<Tuple<bool, IPlayer?>> TryGetPlayerBySsoAsync(INetworkObject networkObject, string sso);
    bool TryAddPlayer(IPlayer player);
    Task<bool> TryRemovePlayerAsync(int playerId);
    Task MarkPlayerAsOnlineAsync(int id);
    Task ResetSsoTokenForPlayerAsync(int id);
    int Count();
    Task<Tuple<bool, IPlayerData?>> TryGetPlayerData(int playerId);
    Task<Tuple<bool, IPlayerData?>> TryGetPlayerDataByUsername(string username);
}