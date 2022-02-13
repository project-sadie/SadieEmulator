using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class Room : RoomData
{
    public Room(long id, string name, RoomLayout layout, List<RoomUser> users) : base(id, name, layout, users)
    {
    }
}