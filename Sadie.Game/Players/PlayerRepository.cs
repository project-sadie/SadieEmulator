using System.Collections.Concurrent;
using System.Diagnostics;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Players;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking;
using Sadie.Db;
using Sadie.Db.Models.Players;

namespace Sadie.Game.Players;

public class PlayerRepository(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper) : IPlayerRepository
{
    private readonly ConcurrentDictionary<long, IPlayerLogic> _players = new();
    private readonly ConcurrentDictionary<long, string> _playerIdToUsernameCache = new();

    public IPlayerLogic? GetPlayerLogicById(long id) => _players.GetValueOrDefault(id);
    public IPlayerLogic? GetPlayerLogicByUsername(string username) => _players.Values.FirstOrDefault(x => x.Player.Username == username);
    
    public async Task<PlayerDto?> GetPlayerByIdAsync(long id)
    {
        if (_players.TryGetValue(id, out var byId))
        {
            return byId.Player;
        }

        var sw = Stopwatch.StartNew();
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var player = await dbContext
            .Set<Player>()
            .Include(x => x.Data)
            .Include(x => x.AvatarData)
            .Include(x => x.OriginRelationships).ThenInclude(x => x.TargetPlayer)
            .Include(x => x.Bans)
            .Include(x => x.GameSettings)
            .Include(x => x.NavigatorSettings)
            .Include(x => x.OutgoingFriendships)
            .Include(x => x.IncomingFriendships)
            .Include(x => x.Roles)
            .Include(x => x.OutgoingIgnores)
            .Include(x => x.RoomLikes)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id);
        
        var value = mapper.Map<PlayerDto>(player);
        sw.Stop();

        if (sw.Elapsed.TotalMilliseconds > 300)
        {
            Console.WriteLine($"Finding a player {id}, {value.Username} took {sw.Elapsed.TotalMilliseconds}ms");
        }
        return value;
    }
    
    public async Task<PlayerDto?> GetPlayerByUsernameAsync(string username)
    {
        var online = _players.Values.FirstOrDefault(x => x.Player.Username == username);
        
        if (online != null)
        {
            return mapper.Map<PlayerDto>(online);
        }
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var player = await dbContext
            .Set<Player>()
            .Include(x => x.Data)
            .FirstOrDefaultAsync(x => x.Username == username);
        
        return mapper.Map<PlayerDto>(player);
    }

    public ICollection<IPlayerLogic> GetAll() => _players.Values;
    
    public bool TryAddPlayer(IPlayerLogic player) => _players.TryAdd(player.Player.Id, player);

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

    public async Task<List<PlayerDto>> GetPlayersForSearchAsync(string searchQuery, long[] excludeIds)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var players = await dbContext
            .Set<Player>()
            .Include(x => x.AvatarData)
            .Where(x => 
                x.Username.Contains(searchQuery) && 
                !excludeIds.Contains(x.Id))
            .ToListAsync();
        
        return mapper.Map<List<PlayerDto>>(players);
    }

    public async Task<List<PlayerRelationshipDto>> GetRelationshipsForPlayerAsync(long playerId)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var playerRelationships = await dbContext
            .Set<PlayerRelationship>()
            .Where(x => x.OriginPlayerId == playerId || x.TargetPlayerId == playerId)
            .ToListAsync();
        
        return mapper.Map<List<PlayerRelationshipDto>>(playerRelationships);
    }

    public async Task BroadcastDataAsync(AbstractPacketWriter writer)
    {
        foreach (var player in _players.Values)
        {
            await player.NetworkObject!.WriteToStreamAsync(writer);
        }
    }

    public async Task<string?> GetPlayerUsernameByIdAsync(long playerId)
    {
        if (_playerIdToUsernameCache.TryGetValue(playerId, out var username))
        {
            return username;
        }
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        username = dbContext.Players
            .Where(x => x.Id == playerId)
            .Select(x => x.Username).FirstOrDefault();

        if (!string.IsNullOrEmpty(username))
        {
            _playerIdToUsernameCache[playerId] = username;
        }
        
        return username;
    }
}