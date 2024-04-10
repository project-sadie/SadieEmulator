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
    HPoint Point { get; }
    HDirection DirectionHead { get; set; }
    HDirection Direction { get; set; }
    void WalkToPoint(Point point, bool useDiagonal);
    void LookAtPoint(Point point);
    void ApplyFlatCtrlStatus();
    Task RunPeriodicCheckAsync();
    void UpdateLastAction();
    void CheckStatusForCurrentTile();
    bool HasRights();
}