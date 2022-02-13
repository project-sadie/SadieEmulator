using System.Collections.Concurrent;

namespace Sadie.Game.Rooms.Users;

public class RoomUserRepository
{
    private readonly ConcurrentDictionary<long, RoomUser> _users;

    public RoomUserRepository(ConcurrentDictionary<long, RoomUser> users)
    {
        _users = users;
    }

    public ICollection<RoomUser> GetAll() => _users.Values;
    public bool TryAdd(RoomUser user) => _users.TryAdd(user.Id, user);
    public int Count => _users.Count;
}