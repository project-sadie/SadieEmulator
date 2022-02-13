using Sadie.Shared;

namespace Sadie.Game.Rooms.Users;

public class RoomUser
{
    public int Id { get; }
    public HPoint Point { get; }
    
    public RoomUser(int id, HPoint point)
    {
        Id = id;
        Point = point;
    }
}