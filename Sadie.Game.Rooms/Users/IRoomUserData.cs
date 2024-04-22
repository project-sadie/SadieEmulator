using Sadie.Game.Players;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public interface IRoomUserData
{
    HPoint Point { get; }
    HDirection DirectionHead { get; set; }
    HDirection Direction { get; set; }
    PlayerLogic Player { get; }
    Dictionary<string, string> StatusMap { get; }
    DateTime LastAction { get; set; }
    TimeSpan IdleTime { get; }
    bool IsIdle { get; }
    bool NeedsStatusUpdate { get; set; }
    bool IsWalking { get; set; }
}