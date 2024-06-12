using Sadie.API.Game.Players;

namespace Sadie.API.Game.Rooms.Users;

public interface IRoomUserData : IRoomUnitMovementData
{
    public IPlayerLogic Player { get; }
    Dictionary<string, string> StatusMap { get; }
    DateTime LastAction { get; set; }
    TimeSpan IdleTime { get; }
    bool IsIdle { get; }
    bool NeedsStatusUpdate { get; set; }
    bool IsWalking { get; set; }
    bool MoonWalking { get; set; }
}