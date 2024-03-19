using Sadie.Shared.Networking;

namespace Sadie.Game.Players;

public interface IPlayerDao
{
    Task<Tuple<bool, IPlayer?>> TryGetPlayerBySsoTokenAsync(INetworkObject networkObject, string sso);
    Task ResetSsoTokenForPlayerAsync(long id);
    Task CreatePlayerRoomLikeAsync(long playerId, long roomId);
}