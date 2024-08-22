using System.Drawing;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Users;
using Sadie.Enums.Game.Rooms.Unit;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms.Unit;

namespace Sadie.Game.Rooms.Users;

public class RoomUserData : RoomUnit, IRoomUserData
{
    public IPlayerLogic Player { get; }
    public DateTime LastAction { get; set; }
    public TimeSpan IdleTime { get; }
    public bool IsIdle { get; protected set; }
    public bool MoonWalking { get; set; }
    public IRoomUserTrade Trade { get; set; }
    public int TradeStatus { get; set; }
    public int ActiveEffectId { get; set; }
    
    protected RoomUserData(
        int id, 
        IRoomLogic room, 
        Point point, 
        double pointZ, 
        HDirection directionHead, 
        HDirection direction, 
        IPlayerLogic player, 
        TimeSpan idleTime) : 
        base(id, RoomUnitType.User, room, point)
    {
        PointZ = pointZ;
        DirectionHead = directionHead;
        Direction = direction;
        Player = player;
        LastAction = DateTime.Now;
        IdleTime = idleTime;
    }
}