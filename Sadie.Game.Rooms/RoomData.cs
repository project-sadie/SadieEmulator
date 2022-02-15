using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class RoomData : RoomSettings
{
    public long Id { get; }
    public string Name { get; }
    public RoomLayout Layout { get; }
    public RoomUserRepository UserRepository { get; }

    protected RoomData(long id, string name, RoomLayout layout, RoomUserRepository userRepository)
    {
        Id = id;
        Name = name;
        Layout = layout;
        UserRepository = userRepository;
    }
}