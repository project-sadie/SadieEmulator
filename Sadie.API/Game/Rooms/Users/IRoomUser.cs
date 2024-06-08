using System.Drawing;
using Sadie.Shared.Unsorted.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.API.Game.Rooms.Users;

public interface IRoomUser : IRoomUserData
{
    int Id { get; }
    IRoomLogic Room { get; }
    INetworkObject NetworkObject { get; }
    Point Point { get; }
    double PointZ { get; }
    HDirection DirectionHead { get; set; }
    HDirection Direction { get; set; }
    void WalkToPoint(Point point, Action? onReachedGoal = null);
    void LookAtPoint(Point point);
    void ApplyFlatCtrlStatus();
    void AddStatus(string key, string value);
    void RemoveStatuses(params string[] statuses);
    Task RunPeriodicCheckAsync();
    void UpdateLastAction();
    void CheckStatusForCurrentTile();
    bool HasRights();
}