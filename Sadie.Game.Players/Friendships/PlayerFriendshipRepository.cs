namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipRepository : IPlayerFriendshipRepository
{
    private readonly IPlayerFriendshipDao _friendshipDao;

    public PlayerFriendshipRepository(IPlayerFriendshipDao friendshipDao)
    {
        _friendshipDao = friendshipDao;
    }

    public async Task<List<PlayerFriendship>> GetIncomingFriendRequestsForPlayerAsync(int playerId)
    {
        return await _friendshipDao.GetIncomingFriendRequestsForPlayerAsync(playerId);
    }

    public async Task<List<PlayerFriendship>> GetOutgoingFriendRequestsForPlayerAsync(int playerId)
    {
        return await _friendshipDao.GetOutgoingFriendRequestsForPlayerAsync(playerId);
    }

    public async Task<List<PlayerFriendship>> GetFriendsForPlayerAsync(int playerId)
    {
        return await _friendshipDao.GetFriendsForPlayerAsync(playerId);
    }

    public async Task AcceptFriendRequestAsync(int originId, int targetId)
    {
        await _friendshipDao.AcceptFriendRequestAsync(originId, targetId);
    }

    public async Task AcceptFriendRequestAsync(int requestId)
    {
        await _friendshipDao.AcceptFriendRequestAsync(requestId);
    }

    public async Task DeclineFriendRequestAsync(int originId, int targetId)
    {
        await _friendshipDao.DeclineFriendRequestAsync(originId, targetId);
    }

    public async Task DeclineAllFriendRequestsAsync(int targetId)
    {
        await _friendshipDao.DeclineAllFriendRequestsAsync(targetId);
    }

    public async Task CreateFriendRequestAsync(int originId, int targetId)
    {
        await _friendshipDao.CreateFriendRequestAsync(originId, targetId);
    }

    public async Task<bool> DoesRequestExist(int playerId1, int playerId2)
    {
        return await _friendshipDao.DoesRequestExist(playerId1, playerId2);
    }

    public async Task<bool> DoesFriendshipExist(int playerId1, int playerId2)
    {
        return await _friendshipDao.DoesFriendshipExist(playerId1, playerId2);
    }

    public async Task<List<PlayerFriendship>> GetAllRecordsForPlayerAsync(int playerId)
    {
        return await _friendshipDao.GetAllRecordsForPlayerAsync(playerId);
    }

    public async Task DeleteFriendshipAsync(int playerId1, int playerId2)
    {
        await _friendshipDao.DeleteFriendshipAsync(playerId1, playerId2);
    }
}