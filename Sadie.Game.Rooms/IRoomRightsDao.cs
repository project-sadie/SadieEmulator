namespace Sadie.Game.Rooms;

public interface IRoomRightsDao
{
    Task InsertRightsAsync(long roomId, long playerId);
    Task DeleteRightsAsync(long roomId, long playerId);
}