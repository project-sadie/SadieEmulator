namespace Sadie.Game.Players.Friendships;

public interface IPlayerFriendshipRepository
{
    Task<List<PlayerFriendshipData>> GetFriendshipRequestsAsync(long playerId);
}