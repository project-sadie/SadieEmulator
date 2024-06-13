using System.Drawing;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Users;
using Sadie.Enums.Game.Rooms.Unit;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Unit;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public class RoomUserData : RoomUnit, IRoomUserData
{
    public IPlayerLogic Player { get; }
    public DateTime LastAction { get; set; }
    public TimeSpan IdleTime { get; }
    public bool IsIdle { get; protected set; }
    public bool MoonWalking { get; set; }
    
    protected RoomUserData(
        int id, 
        IRoomLogic room, 
        Point point, 
        double pointZ, 
        HDirection directionHead, 
        HDirection direction, 
        PlayerLogic player, 
        TimeSpan idleTime,
        RoomFurnitureItemInteractorRepository interactorRepository) : 
        base(id, RoomUnitType.User, room, point, interactorRepository)
    {
        PointZ = pointZ;
        DirectionHead = directionHead;
        Direction = direction;
        Player = player;
        LastAction = DateTime.Now;
        IdleTime = idleTime;
    }
}