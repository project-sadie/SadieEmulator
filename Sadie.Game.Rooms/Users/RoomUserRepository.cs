using System.Collections.Concurrent;

namespace Sadie.Game.Rooms.Users;

public class RoomUserRepository : ConcurrentDictionary<long, RoomUser>
{
    public RoomUserRepository(ConcurrentDictionary<long, RoomUser> users) : base(users)
    {
    }
    
}