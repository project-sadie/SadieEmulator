using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players.Packets;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players;

public class PlayerRepository(
    ILogger<PlayerRepository> logger,
    SadieContext dbContext,
    IMapper mapper)
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
    
    public async Task<PlayerSsoToken?> GetSsoTokenAsync(string token, TimeSpan delay)
    {
        var expireDt = DateTime.Now.Subtract(delay);
        
        var record = await dbContext
            .PlayerSsoToken
            .FirstOrDefaultAsync(x => 
                x.Token == token && 
                x.ExpiresAt > expireDt && 
                x.UsedAt == null);

        if (record == null)
        {
            return record;
        }
        
        record.UsedAt = DateTime.Now;
        dbContext.PlayerSsoToken.Update(record);

        return record;
    }
    
    public async Task<PlayerLogic?> CreatePlayerInstanceWithIdAsync(INetworkObject networkObject, int playerId)
    {
        var player = await dbContext
            .Set<Player>()
            .Where(x => x.Id == playerId)
            .Include(x => x.Data)
            .Include(x => x.AvatarData)
            .Include(x => x.Tags)
            .Include(x => x.RoomLikes)
            .Include(x => x.Relationships)
            .Include(x => x.NavigatorSettings)
            .Include(x => x.GameSettings)
            .Include(x => x.Badges)
            .Include(x => x.FurnitureItems).ThenInclude(x => x.FurnitureItem)
            .Include(x => x.WardrobeItems)
            .Include(x => x.Roles).ThenInclude(x => x.Permissions)
            .Include(x => x.Subscriptions).ThenInclude(x => x.Subscription)
            .Include(x => x.Respects)
            .Include(x => x.SavedSearches)
            .Include(x => x.OutgoingFriendships)
            .Include(x => x.OutgoingFriendships)
            .Include(x => x.IncomingFriendships)
            .Include(x => x.IncomingFriendships)
            .Include(x => x.MessagesSent)
            .Include(x => x.MessagesReceived)
            .Include(x => x.Rooms).ThenInclude(x => x.Settings)
            .FirstOrDefaultAsync();

        if (player == null)
        {
            return null;
        }

        return mapper.Map<PlayerLogic>(player, opt => 
            opt.AfterMap((src, dest) => dest.NetworkObject = networkObject));
    }

    public bool TryAddPlayer(PlayerLogic player) => _players.TryAdd(player.Id, player);

    public async Task<bool> TryRemovePlayerAsync(int playerId)
    {
        var result = _players.TryRemove(playerId, out var player);

        if (player == null)
        {
            return result;
        }

        await UpdateOnlineStatusAsync(player.Id, false);
        await UpdateMessengerStatusForFriends(player.Id, player.GetMergedFriendships(), false, false);
        await player.DisposeAsync();

        return result;
    }

    public async Task UpdateMessengerStatusForFriends(long playerId, IEnumerable<PlayerFriendship> friendships, bool isOnline, bool inRoom)
    {
        foreach (var friendId in friendships.Select(x => x.TargetPlayer.Id).Distinct())
        {
            var friend = GetPlayerLogicById(friendId);
            var friendship = friend?.GetMergedFriendships().FirstOrDefault(x => x.TargetPlayerId == playerId);

            if (friendship == null)
            {
                continue;
            }
            
            var relationship = friend
                .Relationships
                .FirstOrDefault(x =>
                    x.TargetPlayerId == friendship.OriginPlayerId || x.TargetPlayerId == friendship.TargetPlayerId);

            var updateFriendWriter = new PlayerUpdateFriendWriter(
                0, 
                1, 
                0, 
                friendship, 
                isOnline, 
                inRoom, 
                0, 
                "", 
                "", 
                false, 
                false, 
                false,
                relationship?.TypeId ?? PlayerRelationshipType.None);
                
            await friend.NetworkObject.WriteToStreamAsync(updateFriendWriter);
        }
    }
    
    public async Task UpdateOnlineStatusAsync(int playerId, bool isOnline)
    {
        var playerData = await dbContext
            .Set<PlayerData>()
            .FirstOrDefaultAsync(x => x.PlayerId == playerId);

        if (playerData != null)
        {
            playerData.IsOnline = isOnline;
            await dbContext.SaveChangesAsync();
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
            .Where(x => 
                x.Username.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) && 
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