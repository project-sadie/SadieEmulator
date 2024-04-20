using System.Drawing;
using Sadie.Game.Rooms.Enums;
using Sadie.Shared.Unsorted.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Users;

public interface IRoomUser : IRoomUserData, IAsyncDisposable
{
    int Id { get; }
    public RoomControllerLevel ControllerLevel { get; set; }
    INetworkObject NetworkObject { get; }
    Point Point { get; }
    double PointZ { get; }
    HDirection DirectionHead { get; set; }
    HDirection Direction { get; set; }
    void WalkToPoint(Point point);
    void LookAtPoint(Point point);
    void ApplyFlatCtrlStatus();
    void AddStatus(string key, string value);
    Task RunPeriodicCheckAsync();
    void UpdateLastAction();
    void CheckStatusForCurrentTile();
    bool HasRights();
}