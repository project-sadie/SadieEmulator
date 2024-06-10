using System.Drawing;
using Sadie.Enums.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.API.Game.Rooms.Users;

public interface IRoomUser : IRoomUserData, IAsyncDisposable
{
    int Id { get; }
    IRoomLogic Room { get; }
    RoomControllerLevel ControllerLevel { get; set; }
    INetworkObject NetworkObject { get; }
    double PointZ { get; }
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