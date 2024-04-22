using Sadie.Game.Rooms.Enums;
using Sadie.Shared.Unsorted.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Users;

public interface IRoomUser : IRoomUserData, IAsyncDisposable
{
    int Id { get; }
    public RoomLogic Room { get; }
    public RoomControllerLevel ControllerLevel { get; set; }
    INetworkObject NetworkObject { get; }
    void WalkToPoint(HPoint point);
    void LookAtPoint(HPoint point);
    void ApplyFlatCtrlStatus();
    void AddStatus(string key, string value);
    void RemoveStatuses(params string[] statuses);
    Task RunPeriodicCheckAsync();
    void UpdateLastAction();
    void CheckStatusForCurrentTile();
    bool HasRights();
}