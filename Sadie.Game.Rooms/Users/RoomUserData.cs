using Sadie.Shared;

namespace Sadie.Game.Rooms.Users;

public class RoomUserData
{
    public long Id { get; }
    public HPoint Point { get; }
    public HDirection Direction { get; }

    protected RoomUserData(long id, HPoint point, HDirection direction)
    {
        Id = id;
        Point = point;
        Direction = direction;
    }
}