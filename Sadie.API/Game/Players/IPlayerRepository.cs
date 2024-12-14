using Sadie.API.Networking;

namespace Sadie.API.Game.Players;

public interface IPlayerRepository
{
    IPlayerLogic? GetPlayerLogicById(int id);
    IPlayerLogic? GetPlayerLogicByUsername(string username);
    ICollection<IPlayerLogic> GetAll();
    bool TryAddPlayer(IPlayerLogic player);
    Task<bool> TryRemovePlayerAsync(int playerId);
    int Count();
    Task BroadcastDataAsync(AbstractPacketWriter writer);
}