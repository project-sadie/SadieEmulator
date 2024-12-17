using System.Collections.Concurrent;
using Sadie.API.Game.Players;
using Sadie.API.Networking;

namespace Sadie.Game.Players;

public class PlayerRepository : IPlayerRepository
{
    private readonly ConcurrentDictionary<int, IPlayerLogic> _players = new();

    public IPlayerLogic? GetPlayerLogicById(int id) => _players.GetValueOrDefault(id);
    public IPlayerLogic? GetPlayerLogicByUsername(string username) => _players.Values.FirstOrDefault(x => x.Username == username);

    public ICollection<IPlayerLogic> GetAll() => _players.Values;
    
    public bool TryAddPlayer(IPlayerLogic player) => _players.TryAdd(player.Id, player);

    public async Task<bool> TryRemovePlayerAsync(int playerId)
    {
        var result = _players.TryRemove(playerId, out var player);

        if (player == null)
        {
            return result;
        }
        
        await player.DisposeAsync();

        return result;
    }

    public int Count()
    {
        return _players.Count;
    }

    public async Task BroadcastDataAsync(AbstractPacketWriter writer)
    {
        foreach (var player in _players.Values)
        {
            if (player.NetworkObject == null)
            {
                continue;
            }
            
            await player.NetworkObject.WriteToStreamAsync(writer);
        }
    }
}