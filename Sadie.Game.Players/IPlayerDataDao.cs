namespace Sadie.Game.Players;

public interface IPlayerDataDao
{
    Task<Tuple<bool, IPlayerData?>> TryGetPlayerData(long playerId);
    Task<Tuple<bool, IPlayerData?>> TryGetPlayerDataByUsername(string username);
    Task MarkPlayerAsOnlineAsync(long id);
    Task MarkPlayerAsOfflineAsync(IPlayerData playerData);
    Task<List<IPlayerData>> GetPlayerDataForSearch(string searchQuery, int[] excludeIds);
}