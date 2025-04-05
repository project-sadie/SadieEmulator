using Sadie.API.Networking;
using Sadie.Database.Models.Players;

namespace Sadie.API.Game.Players;

public interface IPlayerRepository
{
    IPlayerLogic? GetPlayerLogicById(long id);
    IPlayerLogic? GetPlayerLogicByUsername(string username);
    Task<Player?> GetPlayerByIdAsync(long id);
    Task<Player?> GetPlayerByUsernameAsync(string username);
    ICollection<IPlayerLogic> GetAll();
    bool TryAddPlayer(IPlayerLogic player);
    Task<bool> TryRemovePlayerAsync(long playerId);
    long Count();
    Task<List<Player>> GetPlayersForSearchAsync(string searchQuery, long[] excludeIds);
    Task<List<PlayerRelationship>> GetRelationshipsForPlayerAsync(long playerId);
    Task BroadcastDataAsync(AbstractPacketWriter writer);
}