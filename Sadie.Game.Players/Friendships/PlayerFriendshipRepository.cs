namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipRepository : IPlayerFriendshipRepository
{
    private readonly IPlayerFriendshipDao _friendshipDao;

    public PlayerFriendshipRepository(IPlayerFriendshipDao friendshipDao)
    {
        _friendshipDao = friendshipDao;
    }

    public async Task<List<PlayerFriendshipData>> GetPendingFriendsAsync(long playerId)
    {
        return await _friendshipDao.GetPendingFriendsAsync(playerId);
    }

    public async Task<List<PlayerFriendshipData>> GetActiveFriendsAsync(long playerId)
    {
        return await _friendshipDao.GetActiveFriendsAsync(playerId);
    }
}