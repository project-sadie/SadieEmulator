namespace Sadie.Game.Players.Friendships;

public interface IPlayerFriendshipDao
{
    Task<List<PlayerFriendshipData>> GetFriendshipRecordsAsync(long playerId, PlayerFriendshipStatus status);
    Task<List<PlayerFriendshipData>> GetFriendshipRecordsAsync(long playerId);
    Task<int> GetFriendshipCountAsync(long playerId, PlayerFriendshipStatus status);
    Task<bool> DoesFriendshipExistAsync(long originId, long targetId, PlayerFriendshipStatus status);
    Task CreateAsync(long originId, long targetId, PlayerFriendshipStatus status);
    Task UpdateAsync(long originId, long targetId, PlayerFriendshipStatus newStatus);
}