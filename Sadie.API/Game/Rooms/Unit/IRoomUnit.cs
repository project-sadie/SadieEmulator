using Sadie.Enums.Game.Rooms.Unit;

namespace Sadie.API.Game.Rooms.Unit;

public interface IRoomUnit : IRoomUnitMovementData
{
    int Id { get; }
    RoomUnitType Type { get; set; }
}