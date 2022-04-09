namespace Sadie.Game.Players.Friendships;

public interface IPlayerFriendshipRepository
{
    Task<List<PlayerFriendshipData>> GetFriendshipRecords(long playerId, PlayerFriendshipStatus status);
    Task<int> GetFriendshipCountAsync(long playerId, PlayerFriendshipStatus status);
    Task<bool> DoesFriendshipExistAsync(long player1Id, long player2Id, PlayerFriendshipStatus status);
}