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
    void LookAtPoint(Point point);
    void ApplyFlatCtrlStatus();
    Task RunPeriodicCheckAsync();
    void UpdateLastAction();
    void CheckStatusForCurrentTile();
    bool HasRights();
}