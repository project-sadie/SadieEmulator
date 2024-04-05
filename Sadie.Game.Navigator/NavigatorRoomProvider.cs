using Sadie.Game.Rooms;

namespace Sadie.Game.Navigator;

public class NavigatorRoomProvider(IRoomRepository roomRepository)
{
    public async Task<List<Room>> GetRoomsForCategoryNameAsync(int playerId, string category)
    {
        return category switch
        {
            "popular" => roomRepository.GetPopularRooms(50),
            "my_rooms" => await roomRepository.GetByOwnerIdAsync(playerId, 500),
            _ => []
        };
    }
}