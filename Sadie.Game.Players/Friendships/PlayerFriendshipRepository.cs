namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipRepository : IPlayerFriendshipRepository
{
    private readonly IPlayerFriendshipDao _friendshipDao;

    public PlayerFriendshipRepository(IPlayerFriendshipDao friendshipDao)
    {
        _friendshipDao = friendshipDao;
    }

    public async Task<List<PlayerFriendshipData>> GetFriendshipRequestsAsync(long playerId)
    {
        return await _friendshipDao.GetFriendshipRequestsAsync(playerId);
    }
}