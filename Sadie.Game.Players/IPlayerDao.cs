using Sadie.Game.Players.Relationships;
using Sadie.Shared.Networking;

namespace Sadie.Game.Players;

public interface IPlayerDao
{
    Task<Tuple<bool, IPlayer?>> TryGetPlayerBySsoTokenAsync(INetworkObject networkObject, string sso);
    Task ResetSsoTokenForPlayerAsync(long id);
    Task CreatePlayerRoomLikeAsync(long playerId, long roomId);
    Task<List<PlayerRelationship>> GetRelationshipsAsync(long playerId);
    Task UpdateRelationshipTypeAsync(int id, PlayerRelationshipType type);
    Task<PlayerRelationship> CreateRelationshipAsync(long playerId, long targetPlayerId, PlayerRelationshipType type);
    Task DeleteRelationshipAsync(int id);
    Task CleanDataAsync();
}