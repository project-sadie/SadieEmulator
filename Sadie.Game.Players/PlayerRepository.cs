using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players.DaosToDrop;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Packets;
using Sadie.Game.Players.Relationships;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players;

public class PlayerRepository(
    ILogger<PlayerRepository> logger,
    SadieContext dbContext,
    IMapper mapper)
{
    private readonly ConcurrentDictionary<long, IPlayer> _players = new();

    public bool TryGetPlayerById(long id, out IPlayer? player)
    {
        return _players.TryGetValue(id, out player);
    }

    public bool TryGetPlayerByUsername(string username, out IPlayer? player)
    {
        player = _players.Values.FirstOrDefault(x => x.Data.Username == username);
        return player != default;
    }

    public async Task<IPlayer?> TryGetPlayerBySsoAsync(INetworkObject networkObject, string sso)
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

    public bool TryAddPlayer(IPlayer player) => _players.TryAdd(player.Data.Id, player);

    public async Task<bool> TryRemovePlayerAsync(int playerId)
    {
        var result = _players.TryRemove(playerId, out var player);

        if (player == null)
        {
            return result;
        }

        await UpdateOnlineStatusAsync(player.Data.Id, false);
        await UpdateMessengerStatusForFriends(player.Data.Id, player.Data.FriendshipComponent.Friendships, false, false);
        await player!.DisposeAsync();

        return result;
    }

    public async Task UpdateMessengerStatusForFriends(int playerId, IEnumerable<PlayerFriendship> friendships, bool isOnline, bool inRoom)
    {
        foreach (var friendId in friendships.Select(x => x.TargetData.Id).Distinct())
        {
            if (!TryGetPlayerById(friendId, out var friend) || friend == null)
            {
                continue;
            }
         
            var friendship = friend.Data.FriendshipComponent.Friendships.FirstOrDefault(x => x.TargetData.Id == playerId);

            if (friendship != null)
            {
                var relationship = friend
                    .Data
                    .Relationships
                    .FirstOrDefault(x =>
                        x.TargetPlayerId == friendship.OriginId || x.TargetPlayerId == friendship.TargetId);

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

    public async Task<PlayerData?> TryGetPlayerDataByUsernameAsync(string username)
    {
        return await dbContext
            .Set<Database.Models.Players.PlayerData>()
            .Include(x => x.Player)
            .FirstOrDefaultAsync(x => x.Player.Username == username);
    }

    public async Task<List<PlayerData>> GetPlayerDataForSearchAsync(string searchQuery, int[] excludeIds)
    {
        return await dbContext
            .Set<Database.Models.Players.PlayerData>()
            .Include(x => x.Player)
            .Where(x => x.Player.Username.ToLower().Contains(searchQuery))
            .Where(x => !excludeIds.Contains(x.PlayerId))
            .ToListAsync();
    }

    public ICollection<IPlayer> GetAll() => _players.Values;

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
}