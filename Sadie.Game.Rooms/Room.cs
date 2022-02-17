using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class Room : RoomData
{
    public Room(long id, string name, RoomLayout layout, IRoomUserRepository userRepository) : base(id, name, layout, userRepository)
    {
    }
}