using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Networking;
using Sadie.Database;
using Sadie.Database.Models.Players;

namespace Sadie.Game.Players;

public class PlayerRepository(
    IDbContextFactory<SadieContext> dbContextFactory,
    IMapper mapper) : IPlayerRepository
{
    private readonly ConcurrentDictionary<long, IPlayerLogic> _players = new();

    public IPlayerLogic? GetPlayerLogicById(long id) => _players.GetValueOrDefault(id);
    public IPlayerLogic? GetPlayerLogicByUsername(string username) => _players.Values.FirstOrDefault(x => x.Username == username);
    
    public async Task<Player?> GetPlayerByIdAsync(long id)
    {
        if (_players.TryGetValue(id, out var byId))
        {
            return mapper.Map<Player>(byId);
        }
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        return await dbContext
            .Set<Player>()
            .Include(x => x.Data)
            .Include(x => x.AvatarData)
            .Include(x => x.Relationships).ThenInclude(x => x.TargetPlayer)
            .Include(x => x.Bans)
            .Include(x => x.GameSettings)
            .Include(x => x.NavigatorSettings)
            .Include(x => x.FurnitureItems)
            .Include(x => x.OutgoingFriendships)
            .Include(x => x.IncomingFriendships)
            .Include(x => x.Rooms)
            .Include(x => x.Ignores)
            .Include(x => x.Rooms)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<Player?> GetPlayerByUsernameAsync(string username)
    {
        var online = _players.Values.FirstOrDefault(x => x.Username == username);
        
        if (online != null)
        {
            return mapper.Map<Player>(online);
        }
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        return await dbContext
            .Set<Player>()
            .Include(x => x.Data)
            .FirstOrDefaultAsync(x => x.Username == username);
    }

    public ICollection<IPlayerLogic> GetAll() => _players.Values;
    
    public bool TryAddPlayer(IPlayerLogic player) => _players.TryAdd(player.Id, player);

    public async Task<bool> TryRemovePlayerAsync(long playerId)
    {
        var result = _players.TryRemove(playerId, out var player);

        if (player == null)
        {
            return result;
        }
        
        await player.DisposeAsync();

        return result;
    }

    public long Count()
    {
        return _players.Count;
    }

    public async Task<List<Player>> GetPlayersForSearchAsync(string searchQuery, long[] excludeIds)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        return await dbContext
            .Set<Player>()
            .Include(x => x.AvatarData)
            .Where(x => 
                x.Username.Contains(searchQuery) && 
                !excludeIds.Contains(x.Id))
            .ToListAsync();
    }

    public async Task<List<PlayerRelationship>> GetRelationshipsForPlayerAsync(long playerId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        return await dbContext
            .Set<PlayerRelationship>()
            .Where(x => x.OriginPlayerId == playerId || x.TargetPlayerId == playerId)
            .ToListAsync();
    }

    public async Task BroadcastDataAsync(AbstractPacketWriter writer)
    {
        foreach (var player in _players.Values)
        {
            await player.NetworkObject.WriteToStreamAsync(writer);
        }
    }
}