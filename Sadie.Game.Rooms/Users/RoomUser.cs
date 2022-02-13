using Sadie.Shared;

namespace Sadie.Game.Rooms.Users;

public class RoomUser
{
    public long Id { get; }
    public HPoint Point { get; }
    
    public RoomUser(long id, HPoint point)
    {
        Id = id;
        Point = point;
    }
}