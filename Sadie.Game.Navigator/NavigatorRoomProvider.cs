using Sadie.Game.Rooms;

namespace Sadie.Game.Navigator;

public class NavigatorRoomProvider(RoomRepository roomRepository)
{
    public async Task<List<RoomLogic>> GetRoomsForCategoryNameAsync(int playerId, string category)
    {
        return category switch
        {
            "popular" => roomRepository.GetPopularRooms(50),
            "my_rooms" => await roomRepository.GetAllByOwnerIdAsync(playerId, 500),
            _ => []
        };
    }
}