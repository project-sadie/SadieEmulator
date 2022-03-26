using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class RoomData : RoomSettings
{
    public long Id { get; }
    public string Name { get; }
    public RoomLayout Layout { get; }
    public long OwnerId { get; }
    public string OwnerName { get; }
    public IRoomUserRepository UserRepository { get; }

    protected RoomData(long id, string name, RoomLayout layout, long ownerId, string ownerName, IRoomUserRepository userRepository, bool walkDiagonal) : base(walkDiagonal)
    {
        Id = id;
        Name = name;
        Layout = layout;
        OwnerId = ownerId;
        OwnerName = ownerName;
        UserRepository = userRepository;
    }
}