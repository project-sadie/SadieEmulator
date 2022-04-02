using Sadie.Game.Rooms;

namespace Sadie.Game.Navigator;

public class NavigatorRoomProvider
{
    private readonly IRoomRepository _roomRepository;

    public NavigatorRoomProvider(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    public List<IRoom> GetRoomsForCategoryName(string category)
    {
        return category switch
        {
            "popular" => _roomRepository.GetPopularRooms(50),
            _ => new List<IRoom>()
        };
    }
}