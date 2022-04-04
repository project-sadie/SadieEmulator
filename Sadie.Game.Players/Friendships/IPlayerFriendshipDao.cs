namespace Sadie.Game.Players.Friendships;

public interface IPlayerFriendshipDao
{
    Task<List<PlayerFriendshipData>> GetFriendshipRecordsAsync(long playerId, PlayerFriendshipStatus status);
    Task<int> GetFriendshipCountAsync(long playerId, PlayerFriendshipStatus status);
    Task<bool> DoesFriendshipExistAsync(long player1Id, long player2Id, PlayerFriendshipStatus status);
}