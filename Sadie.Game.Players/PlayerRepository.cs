using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players.Packets;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players;

public class PlayerRepository(
    ILogger<PlayerRepository> logger,
    SadieContext dbContext,
    IMapper mapper)
{
    private readonly ConcurrentDictionary<int, PlayerLogic> _players = new();

    public bool TryGetPlayerById(int id, out PlayerLogic? player)
    {
        return _players.TryGetValue(id, out player);
    }

    public bool TryGetPlayerByUsername(string username, out PlayerLogic? player)
    {
        player = _players.Values.FirstOrDefault(x => x.Username == username);
        return player != default;
    }

    public async Task<PlayerLogic?> TryGetPlayerBySsoAsync(INetworkObject networkObject, string sso)
    {
        var player = await dbContext
            .Set<Player>()
            .Where(x => x.SsoToken == sso)
            .FirstOrDefaultAsync();

        if (player == null)
        {
            return null;
        }
        
        player.SsoToken = null;
        await dbContext.SaveChangesAsync();

        return mapper.Map<PlayerLogic>(player);
    }

    public bool TryAddPlayer(PlayerLogic player) => _players.TryAdd(player.Data.Id, player);

    public async Task<bool> TryRemovePlayerAsync(int playerId)
    {
        var result = _players.TryRemove(playerId, out var player);

        if (player == null)
        {
            return result;
        }

        await UpdateOnlineStatusAsync(player.Data.Id, false);
        await UpdateMessengerStatusForFriends(player.Data.Id, player.Friendships, false, false);
        await player!.DisposeAsync();

        return result;
    }

    public async Task UpdateMessengerStatusForFriends(long playerId, IEnumerable<PlayerFriendship> friendships, bool isOnline, bool inRoom)
    {
        foreach (var friendId in friendships.Select(x => x.TargetPlayer.Id).Distinct())
        {
            if (!TryGetPlayerById(friendId, out var friend) || friend == null)
            {
                continue;
            }
         
            var friendship = friend.Friendships.FirstOrDefault(x => x.TargetPlayerId == playerId);

            if (friendship != null)
            {
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
                        relationship?.Type ?? PlayerRelationshipType.None).GetAllBytes();
                
                await friend.NetworkObject.WriteToStreamAsync(updateFriendWriter);
            }
        }
    }
    
    public async Task UpdateOnlineStatusAsync(int playerId, bool isOnline)
    {
        var playerData = await dbContext
            .Set<Database.Models.Players.PlayerData>()
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

    public async Task<PlayerData?> TryGetPlayerDataAsync(int playerId)
    {
        return await dbContext
            .Set<Database.Models.Players.PlayerData>()
            .FirstOrDefaultAsync(x => x.PlayerId == playerId);
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

    public ICollection<PlayerLogic> GetAll() => _players.Values;

    public async ValueTask DisposeAsync()
    {
        foreach (var player in _players.Values)
        {
            if (!await TryRemovePlayerAsync(player.Data.Id))
            {
                logger.LogError("Failed to dispose of player");
            }
        }
    }

    public async Task<List<PlayerRelationship>> GetRelationshipsForPlayerAsync(int playerId)
    {
        throw new NotImplementedException();
    }

    public async Task<Player?> TryGetPlayerByUsernameAsync(string username)
    {
        return await dbContext
            .Set<Player>()
            .Include(x => x.Data)
            .FirstOrDefaultAsync(x => x.Username == username);
    }
}