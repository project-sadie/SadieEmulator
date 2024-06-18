using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Players;

public class PlayerRepository(
    ILogger<PlayerRepository> logger,
    SadieContext dbContext)
{
    private readonly ConcurrentDictionary<int, PlayerLogic> _players = new();

    public PlayerLogic? GetPlayerLogicById(int id) => _players.GetValueOrDefault(id);
    public PlayerLogic? GetPlayerLogicByUsername(string username) => _players.Values.FirstOrDefault(x => x.Username == username);
    
    public async Task<Player?> GetPlayerByIdAsync(int id)
    {
        if (_players.TryGetValue(id, out var byId))
        {
            return byId;
        }
        
        return await dbContext
            .Set<Player>()
            .Include(x => x.Data)
            .Include(x => x.AvatarData)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<Player?> GetPlayerByUsernameAsync(string username)
    {
        var online = _players.Values.FirstOrDefault(x => x.Username == username);
        
        if (online != null)
        {
            return online;
        }
        
        return await dbContext
            .Set<Player>()
            .Include(x => x.Data)
            .FirstOrDefaultAsync(x => x.Username == username);
    }

    public ICollection<PlayerLogic> GetAll() => _players.Values;
    
    public bool TryAddPlayer(PlayerLogic player) => _players.TryAdd(player.Id, player);

    public async Task<bool> TryRemovePlayerAsync(int playerId)
    {
        var result = _players.TryRemove(playerId, out var player);

        if (player == null)
        {
            return result;
        }

        player.Data.IsOnline = false;
        dbContext.Entry(player.Data).Property(x => x.IsOnline).IsModified = true;
        await dbContext.SaveChangesAsync();
        
        await UpdateStatusForFriendsAsync(player, player.GetMergedFriendships(), false, false);
        await player.DisposeAsync();

        return result;
    }

    public async Task UpdateStatusForFriendsAsync(
        Player player, 
        IEnumerable<PlayerFriendship> friendships, 
        bool isOnline, 
        bool inRoom)
    {
        var update = new PlayerFriendshipUpdate
        {
            Type = 0,
            Friend = player,
            FriendOnline = isOnline,
            FriendInRoom = inRoom,
            Relation = PlayerRelationshipType.None
        };
        
        foreach (var friend in friendships)
        {
            var targetId = friend.OriginPlayerId == player.Id ? 
                friend.TargetPlayerId : 
                friend.OriginPlayerId;

            var targetPlayer = GetPlayerLogicById(targetId);

            if (targetPlayer != null)
            {
                await PlayerFriendshipHelpers.SendFriendUpdatesToPlayerAsync(targetPlayer, [update]);
            }
        }
    }

    public int Count()
    {
        return _players.Count;
    }

    public async Task<List<Player>> GetPlayersForSearchAsync(string searchQuery, int[] excludeIds)
    {
        return await dbContext
            .Set<Player>()
            .Include(x => x.AvatarData)
            .Where(x => 
                x.Username.Contains(searchQuery) && 
                !excludeIds.Contains(x.Id))
            .ToListAsync();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var player in _players.Values)
        {
            if (!await TryRemovePlayerAsync(player.Id))
            {
                logger.LogError("Failed to dispose of player");
            }
        }
    }

    public async Task<List<PlayerRelationship>> GetRelationshipsForPlayerAsync(int playerId)
    {
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