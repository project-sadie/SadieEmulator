using System.Drawing;
using Sadie.API.Game.Players;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.API.Game.Rooms.Users;

public interface IRoomUserData
{
    Point Point { get; }
    HDirection DirectionHead { get; set; }
    HDirection Direction { get; set; }
    public IPlayerLogic Player { get; }
    Dictionary<string, string> StatusMap { get; }
    DateTime LastAction { get; set; }
    TimeSpan IdleTime { get; }
    bool IsIdle { get; }
    bool NeedsStatusUpdate { get; set; }
    bool IsWalking { get; set; }
    bool MoonWalking { get; set; }
}