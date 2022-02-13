using System.Collections.Concurrent;

namespace Sadie.Game.Rooms.Users;

public class RoomUserRepository
{
    private readonly ConcurrentDictionary<long, RoomUser> _users;

    public RoomUserRepository(ConcurrentDictionary<long, RoomUser> users)
    {
        _users = users;
    }
}