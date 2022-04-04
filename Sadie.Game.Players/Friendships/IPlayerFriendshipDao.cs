namespace Sadie.Game.Players.Friendships;

public interface IPlayerFriendshipDao
{
    Task<List<PlayerFriendshipData>> GetPendingFriendsAsync(long playerId);
    Task<List<PlayerFriendshipData>> GetActiveFriendsAsync(long playerId);
    Task<int> GetActiveFriendsCountAsync(long playerId);
    Task<bool> DoesFriendshipExist(long player1Id, long player2Id, PlayerFriendshipStatus status);
}