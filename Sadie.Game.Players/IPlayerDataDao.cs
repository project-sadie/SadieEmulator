namespace Sadie.Game.Players;

public interface IPlayerDataDao
{
    Task<Tuple<bool, PlayerData?>> TryGetPlayerData(long playerId);
    Task<Tuple<bool, PlayerData?>> TryGetPlayerDataByUsername(string username);
    Task MarkPlayerAsOnlineAsync(long id);
    Task MarkPlayerAsOfflineAsync(PlayerData data, IPlayerState state);
    Task<List<PlayerData>> GetPlayerDataForSearch(string searchQuery, int[] excludeIds);
}