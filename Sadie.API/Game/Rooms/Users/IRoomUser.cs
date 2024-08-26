using System.Drawing;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Enums.Game.Rooms;

namespace Sadie.API.Game.Rooms.Users;

public interface IRoomUser : IRoomUnit, IAsyncDisposable
{
    IPlayerLogic Player { get; }
    DateTime LastAction { get; set; }
    TimeSpan IdleTime { get; }
    bool IsIdle { get; set; }
    bool MoonWalking { get; set; }
    IRoomUserTrade? Trade { get; set; }
    int TradeStatus { get; set; }
    int ActiveEffectId { get; set; }
    IRoomLogic Room { get; }
    RoomControllerLevel ControllerLevel { get; set; }
    INetworkObject NetworkObject { get; }
    void LookAtPoint(Point point);
    void ApplyFlatCtrlStatus();
    void CheckStatusForCurrentTile();
    bool HasRights();
    Task SendWhisperAsync(string message);
}