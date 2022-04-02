namespace Sadie.Game.Players.Friendships;

public interface IPlayerFriendshipRepository
{
    Task<List<PlayerFriendshipData>> GetPendingFriendsAsync(long playerId);
    Task<List<PlayerFriendshipData>> GetActiveFriendsAsync(long playerId);
}