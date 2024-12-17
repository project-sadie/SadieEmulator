using Sadie.Database.Models.Rooms;

namespace Sadie.API.Game.Rooms;

public interface IRoomRepository
{
    IRoomLogic? TryGetRoomById(int id);
    void AddRoom(IRoomLogic roomLogic);
    List<Room> GetPopularRooms(int amount);
    int Count { get; }
    IEnumerable<IRoomLogic> GetAllRooms();
    bool TryRemove(int id, out IRoomLogic? roomLogic);
    ValueTask DisposeAsync();
}