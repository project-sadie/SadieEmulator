using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public interface IRoomUserData
{
    HPoint Point { get; }
    HDirection DirectionHead { get; }
    HDirection Direction { get; }
    AvatarData AvatarData { get; }
    Dictionary<string, string> StatusMap { get; }
    DateTime LastAction { get; }
    bool IsIdle { get; }
}