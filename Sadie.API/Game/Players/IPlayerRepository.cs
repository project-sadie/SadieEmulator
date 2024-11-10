using Sadie.API.Networking;
using Sadie.Database.Models.Players;

namespace Sadie.API.Game.Players;

public interface IPlayerRepository
{
    IPlayerLogic? GetPlayerLogicById(int id);
    IPlayerLogic? GetPlayerLogicByUsername(string username);
    Task<Player?> GetPlayerByIdAsync(int id);
    Task<Player?> GetPlayerByUsernameAsync(string username);
    ICollection<IPlayerLogic> GetAll();
    bool TryAddPlayer(IPlayerLogic player);
    Task<bool> TryRemovePlayerAsync(int playerId);
    int Count();
    Task<List<Player>> GetPlayersForSearchAsync(string searchQuery, int[] excludeIds);
    Task<List<PlayerRelationship>> GetRelationshipsForPlayerAsync(int playerId);
    Task BroadcastDataAsync(AbstractPacketWriter writer);
}