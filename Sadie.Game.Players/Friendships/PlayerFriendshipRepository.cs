namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipRepository : IPlayerFriendshipRepository
{
    private readonly IPlayerFriendshipDao _friendshipDao;

    public PlayerFriendshipRepository(IPlayerFriendshipDao friendshipDao)
    {
        _friendshipDao = friendshipDao;
    }

    public async Task<List<PlayerFriendshipData>> GetFriendshipRecords(long playerId, PlayerFriendshipStatus status)
    {
        return await _friendshipDao.GetFriendshipRecordsAsync(playerId, status);
    }

    public async Task<int> GetFriendsCountAsync(long playerId, PlayerFriendshipStatus status)
    {
        return await _friendshipDao.GetFriendshipCountAsync(playerId, status);
    }

    public async Task<bool> ExistsAsync(long player1Id, long player2Id, PlayerFriendshipStatus status)
    {
        return await _friendshipDao.DoesFriendshipExistAsync(player1Id, player2Id, status);
    }

    public async Task CreateAsync(long player1Id, long player2Id, PlayerFriendshipStatus status)
    {
        await _friendshipDao.CreateAsync(player1Id, player2Id, status);
    }

    public Task UpdateAsync(long player1Id, long player2Id, PlayerFriendshipStatus newStatus)
    {
        throw new NotImplementedException();
    }
}