namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipRepository(IPlayerFriendshipDao friendshipDao) : IPlayerFriendshipRepository
{
    public async Task<List<PlayerFriendship>> GetIncomingFriendRequestsForPlayerAsync(int playerId)
    {
        return await friendshipDao.GetIncomingFriendRequestsForPlayerAsync(playerId);
    }

    public async Task<List<PlayerFriendship>> GetOutgoingFriendRequestsForPlayerAsync(int playerId)
    {
        return await friendshipDao.GetOutgoingFriendRequestsForPlayerAsync(playerId);
    }

    public async Task<List<PlayerFriendship>> GetFriendsForPlayerAsync(int playerId)
    {
        return await friendshipDao.GetFriendsForPlayerAsync(playerId);
    }

    public async Task AcceptFriendRequestAsync(int originId, int targetId)
    {
        await friendshipDao.AcceptFriendRequestAsync(originId, targetId);
    }

    public async Task AcceptFriendRequestAsync(int requestId)
    {
        await friendshipDao.AcceptFriendRequestAsync(requestId);
    }

    public async Task DeclineFriendRequestAsync(int originId, int targetId)
    {
        await friendshipDao.DeclineFriendRequestAsync(originId, targetId);
    }

    public async Task DeclineAllFriendRequestsAsync(int targetId)
    {
        await friendshipDao.DeclineAllFriendRequestsAsync(targetId);
    }

    public async Task CreateFriendRequestAsync(int originId, int targetId)
    {
        await friendshipDao.CreateFriendRequestAsync(originId, targetId);
    }

    public async Task<bool> DoesRequestExist(int playerId1, int playerId2)
    {
        return await friendshipDao.DoesFriendRequestExist(playerId1, playerId2);
    }

    public async Task<bool> DoesFriendshipExist(int playerId1, int playerId2)
    {
        return await friendshipDao.DoesFriendshipExist(playerId1, playerId2);
    }

    public async Task<List<PlayerFriendship>> GetAllRecordsForPlayerAsync(int playerId)
    {
        return await friendshipDao.GetAllRecordsForPlayerAsync(playerId);
    }

    public async Task DeleteFriendshipAsync(int playerId1, int playerId2)
    {
        await friendshipDao.DeleteFriendshipAsync(playerId1, playerId2);
    }
}