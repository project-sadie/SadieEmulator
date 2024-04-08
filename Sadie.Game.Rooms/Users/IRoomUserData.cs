using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public interface IRoomUserData
{
    HPoint Point { get; }
    HDirection DirectionHead { get; }
    HDirection Direction { get; }
    PlayerLogic Player { get; }
    Dictionary<string, string> StatusMap { get; }
    DateTime LastAction { get; set; }
    TimeSpan IdleTime { get; }
    bool IsIdle { get; }
}