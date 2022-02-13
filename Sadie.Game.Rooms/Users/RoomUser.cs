using Sadie.Shared;

namespace Sadie.Game.Rooms.Users;

public class RoomUser : RoomUserData
{
    public RoomUser(long id, HPoint point, HDirection direction) : base(id, point, direction)
    {
    }
}