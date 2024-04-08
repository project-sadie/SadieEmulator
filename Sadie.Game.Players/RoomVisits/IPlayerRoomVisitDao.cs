namespace Sadie.Game.Players.RoomVisits;

public interface IPlayerRoomVisitDao
{
    Task<int> CreateAsync(List<PlayerRoomVisit> roomVisits);
}