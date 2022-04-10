using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class RoomData
{
    public int Id { get; }
    public string Name { get; }
    public RoomLayout Layout { get; }
    public int OwnerId { get; }
    public string OwnerName { get; }
    public string Description { get; }
    public int Score { get; }
    public List<string> Tags { get; }
    public int MaxUsers { get; }
    public IRoomUserRepository UserRepository { get; }
    public IRoomSettings Settings { get; }

    protected RoomData(int id,
        string name,
        RoomLayout layout,
        int ownerId,
        string ownerName,
        string description,
        int score,
        List<string> tags,
        int maxUsers,
        IRoomUserRepository userRepository,
        IRoomSettings settings) 
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
        Settings = settings;
    }
}