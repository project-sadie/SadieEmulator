using Sadie.Game.Rooms;

namespace Sadie.Game.Navigator;

public class NavigatorRoomProvider(IRoomRepository roomRepository)
{
    public List<IRoom> GetRoomsForCategoryName(string category)
    {
        return category switch
        {
            "popular" => roomRepository.GetPopularRooms(50),
            _ => new List<IRoom>()
        };
    }
}