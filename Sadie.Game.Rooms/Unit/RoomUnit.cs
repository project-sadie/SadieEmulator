using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Enums.Game.Rooms.Unit;
using Sadie.Game.Rooms.Furniture;

namespace Sadie.Game.Rooms.Unit;

public class RoomUnit : 
    RoomUnitMovementData, IRoomUnit
{
    protected RoomUnit(int id, RoomUnitType type, 
        IRoomLogic room, 
        Point point, 
        RoomFurnitureItemInteractorRepository interactorRepository) : base(room, point, interactorRepository)
    {
        Id = id;
        Type = type;

        SetUnitOnBase();
    }

    public int Id { get; }
    public RoomUnitType Type { get; set; }

    public void SetUnitOnBase() => Unit = this;
}