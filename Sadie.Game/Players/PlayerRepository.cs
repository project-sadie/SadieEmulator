using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Player;
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

    public IPlayerLogic? GetPlayerLogicById(long id) => _players.GetValueOrDefault(id);
    public IPlayerLogic? GetPlayerLogicByUsername(string username) => _players.Values.FirstOrDefault(x => x.Player.Username == username);
    
    public async Task<PlayerDto?> GetPlayerByIdAsync(long id)
    {
        if (_players.TryGetValue(id, out var byId))
        {
            return mapper.Map<PlayerDto>(byId);
        }
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var player = await dbContext
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
            .Include(x => x.Roles)
            .Include(x => x.Ignores)
            .Include(x => x.Rooms)
            .Include(x => x.RoomLikes)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id);
        
        return mapper.Map<PlayerDto>(player);
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
}