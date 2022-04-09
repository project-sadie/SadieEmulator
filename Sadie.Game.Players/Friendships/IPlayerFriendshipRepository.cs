namespace Sadie.Game.Players.Friendships;

public interface IPlayerFriendshipRepository
{
    Task<List<PlayerFriendshipData>> GetFriendshipRecords(long playerId, PlayerFriendshipStatus status);
    Task<int> GetFriendsCountAsync(long playerId, PlayerFriendshipStatus status);
    Task<bool> ExistsAsync(long player1Id, long player2Id, PlayerFriendshipStatus status);

    Task CreateAsync(long player1Id, long player2Id, PlayerFriendshipStatus status);
    Task UpdateAsync(long player1Id, long player2Id, PlayerFriendshipStatus newStatus);
}