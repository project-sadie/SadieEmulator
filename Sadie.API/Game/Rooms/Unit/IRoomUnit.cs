namespace Sadie.API.Game.Rooms.Unit;

public interface IRoomUnit : IRoomUnitData
{
    ValueTask RunPeriodicCheckAsync();
}