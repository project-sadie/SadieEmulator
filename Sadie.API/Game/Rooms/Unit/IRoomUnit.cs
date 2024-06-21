using Sadie.Enums.Game.Rooms.Unit;

namespace Sadie.API.Game.Rooms.Unit;

public interface IRoomUnit : IRoomUnitData
{
    int Id { get; }
    RoomUnitType Type { get; set; }
}