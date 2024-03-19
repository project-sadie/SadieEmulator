using Sadie.Game.Players.Friendships;
using Sadie.Shared.Networking;

namespace Sadie.Game.Players;

public interface IPlayerRepository : IAsyncDisposable
{
    bool TryGetPlayerById(long id, out IPlayer? player);
    bool TryGetPlayerByUsername(string username, out IPlayer? player);
    Task<Tuple<bool, IPlayer?>> TryGetPlayerBySsoAsync(INetworkObject networkObject, string sso);
    bool TryAddPlayer(IPlayer player);
    Task<bool> TryRemovePlayerAsync(int playerId);
    Task UpdateMessengerStatusForFriends(int playerId, IEnumerable<PlayerFriendship> friendships, bool isOnline, bool inRoom);
    Task MarkPlayerAsOnlineAsync(int id);
    Task ResetSsoTokenForPlayerAsync(int id);
    int Count();
    Task<Tuple<bool, IPlayerData?>> TryGetPlayerDataAsync(int playerId);
    Task<Tuple<bool, IPlayerData?>> TryGetPlayerDataByUsernameAsync(string username);
    Task<List<IPlayerData>> GetPlayerDataForSearchAsync(string searchQuery, int[] excludeIds);
}