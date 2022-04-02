namespace Sadie.Game.Players.Friendships;

public interface IPlayerFriendshipDao
{
    Task<List<PlayerFriendshipData>> GetPendingFriendsAsync(long playerId);
    Task<List<PlayerFriendshipData>> GetActiveFriendsAsync(long playerId);
}