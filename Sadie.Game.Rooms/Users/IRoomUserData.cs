using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public interface IRoomUserData
{
    HPoint Point { get; }
    HDirection DirectionHead { get; }
    HDirection Direction { get; }
    AvatarData AvatarData { get; }
    Dictionary<string, string> StatusMap { get; }
    DateTime LastAction { get; set; }
    TimeSpan IdleTime { get; }
    bool IsIdle { get; }
}