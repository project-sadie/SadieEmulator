namespace Sadie.Game.Players.Room;

public interface IPlayerRoomVisitDao
{
    Task<int> CreateAsync(List<PlayerRoomVisit> roomVisits);
}