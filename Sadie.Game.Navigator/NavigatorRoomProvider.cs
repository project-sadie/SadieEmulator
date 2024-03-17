using Sadie.Game.Rooms;

namespace Sadie.Game.Navigator;

public class NavigatorRoomProvider(IRoomRepository roomRepository)
{
    public async Task<List<IRoom>> GetRoomsForCategoryNameAsync(int playerId, string category)
    {
        return category switch
        {
            "popular" => roomRepository.GetPopularRooms(50),
            "myrooms" => await roomRepository.GetByOwnerIdAsync(playerId, 500),
            _ => new List<IRoom>()
        };
    }
}