namespace Sadie.Game.Players.Friendships;

public interface IPlayerFriendshipRepository
{
    Task<List<PlayerFriendship>> GetIncomingFriendRequestsForPlayerAsync(int playerId);
    Task<List<PlayerFriendship>> GetOutgoingFriendRequestsForPlayerAsync(int playerId);
    Task<List<PlayerFriendship>> GetFriendsForPlayerAsync(int playerId);
    Task AcceptFriendRequestAsync(int originId, int targetId);
    Task AcceptFriendRequestAsync(int requestId);
    Task DeclineFriendRequestAsync(int originId, int targetId);
    Task DeclineAllFriendRequestsAsync(int targetId);
    Task CreateFriendRequestAsync(int originId, int targetId);
    Task<bool> DoesRequestExist(int playerId1, int playerId2);
    Task<bool> DoesFriendshipExist(int playerId1, int playerId2);
    Task<List<PlayerFriendship>> GetAllRecordsForPlayerAsync(int playerId);
    Task DeleteFriendshipAsync(int playerId1, int playerId2);
}