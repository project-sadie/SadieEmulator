using Sadie.Game.Rooms.Chat;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class RoomData : IRoomData
{
    public int Id { get; }
    public string Name { get; set; }
    public RoomLayout Layout { get; }
    public int OwnerId { get; }
    public string OwnerName { get; }
    public string Description { get; set; }
    public int Score { get; }
    public List<string> Tags { get; set; }
    public int MaxUsers { get; set; }
    public bool Muted { get; }
    public IRoomUserRepository UserRepository { get; }
    public IRoomSettings Settings { get; }
    public List<RoomChatMessage> ChatMessages { get; }
    public List<int> PlayersWithRights { get; }

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
        IRoomSettings settings,
        List<int> playersWithRights) 
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
        ChatMessages = new List<RoomChatMessage>();
        PlayersWithRights = playersWithRights;
    }
}