using Sadie.Database.Models.Rooms;
using Sadie.Game.Players;
using Sadie.Game.Rooms;

namespace Sadie.Game.Navigator;

public class NavigatorRoomProvider(RoomRepository roomRepository)
{
    public Task<List<Room>> GetRoomsForCategoryNameAsync(PlayerLogic player, string category)
    {
        return Task.FromResult(category switch
        {
            "popular" => roomRepository.GetPopularRooms(50),
            "my_rooms" => player.Rooms.ToList(),
            _ => []
        });
    }
}