using Sadie.Game.Rooms;

namespace Sadie.Game.Navigator;

public class NavigatorRoomProvider(IRoomRepository roomRepository)
{
    public List<IRoom> GetRoomsForCategoryName(int playerId, string category)
    {
        return category switch
        {
            "popular" => roomRepository.GetPopularRooms(50),
            "myrooms" => roomRepository.GetByOwnerId(playerId, 500),
            _ => new List<IRoom>()
        };
    }
}