using System.Collections.Concurrent;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class RoomData : RoomSettings
{
    public long Id { get; }
    public string Name { get; }
    public RoomLayout Layout { get; }
    public ConcurrentDictionary<long, RoomUser> Users { get; }

    protected RoomData(long id, string name, RoomLayout layout, ConcurrentDictionary<long, RoomUser> users)
    {
        Id = id;
        Name = name;
        Layout = layout;
        Users = users;
    }
}