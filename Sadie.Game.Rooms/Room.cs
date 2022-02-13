using System.Collections.Concurrent;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class Room : RoomData
{
    public Room(long id, string name, RoomLayout layout, RoomUserRepository userRepository) : base(id, name, layout, userRepository)
    {
    }
}