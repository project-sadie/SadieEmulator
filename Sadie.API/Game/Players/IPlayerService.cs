using Sadie.Database.Models.Players;

namespace Sadie.API.Game.Players;

public interface IPlayerService
{
    Task<PlayerSsoToken?> GetTokenAsync(string token, int delayMs);
    Task<Player?> GetPlayerByIdAsync(int id, bool checkInMemory = true);
    Task<List<PlayerRelationship>> GetRelationshipsForPlayerAsync(int playerId);
    Task<List<Player>> GetPlayersForSearchAsync(string searchQuery, int[] excludeIds);
    Task<Player?> GetPlayerByUsernameAsync(string username, bool checkInMemory = true);
}