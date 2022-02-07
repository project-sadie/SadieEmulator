namespace Sadie.Game.Players.Friendships;

public interface IPlayerFriendshipDao
{
    Task<List<PlayerFriendshipData>> GetFriendshipRequestsAsync(long playerId);
}