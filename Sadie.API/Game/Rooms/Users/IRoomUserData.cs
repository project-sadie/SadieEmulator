using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms.Unit;

namespace Sadie.API.Game.Rooms.Users;

public interface IRoomUserData : IRoomUnit
{
    public IPlayerLogic Player { get; }
    Dictionary<string, string> StatusMap { get; }
    DateTime LastAction { get; set; }
    TimeSpan IdleTime { get; }
    bool IsIdle { get; }
    bool NeedsStatusUpdate { get; set; }
    bool IsWalking { get; set; }
    bool MoonWalking { get; set; }
    int HandItemId { get; set; }
    IRoomUserTrade? Trade { get; set; }
    int TradeStatus { get; set; }
}