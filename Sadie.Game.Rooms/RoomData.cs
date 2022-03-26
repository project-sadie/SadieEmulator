using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class RoomData : RoomSettings
{
    public long Id { get; }
    public string Name { get; }
    public RoomLayout Layout { get; }
    public long OwnerId { get; }
    public string OwnerName { get; }
    public string Description { get; }
    public int Score { get; }
    public List<string> Tags { get; }
    public int MaxUsers { get; }
    public IRoomUserRepository UserRepository { get; }

    protected RoomData(long id, string name, RoomLayout layout, long ownerId, string ownerName, string description, int score, List<string> tags, int maxUsers, IRoomUserRepository userRepository, bool walkDiagonal) : base(walkDiagonal)
    {
        Id = id;
        Name = name;
        Layout = layout;
        OwnerId = ownerId;
        OwnerName = ownerName;
        Description = description;
        Score = score;
        Tags = tags;
        MaxUsers = maxUsers;
        UserRepository = userRepository;
    }
}