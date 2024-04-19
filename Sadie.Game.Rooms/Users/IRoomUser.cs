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
    void WalkToPoint(HPoint point);
    void LookAtPoint(HPoint point);
    void ApplyFlatCtrlStatus();
    void AddStatus(string key, string value);
    Task RunPeriodicCheckAsync();
    void UpdateLastAction();
    void CheckStatusForCurrentTile();
    bool HasRights();
}