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

    public async Task<int> GetActiveFriendsCountAsync(long playerId)
    {
        return await _friendshipDao.GetActiveFriendsCountAsync(playerId);
    }

    public async Task<bool> DoesFriendshipExist(long player1Id, long player2Id, PlayerFriendshipStatus status)
    {
        return await _friendshipDao.DoesFriendshipExist(player1Id, player2Id, status);
    }
}