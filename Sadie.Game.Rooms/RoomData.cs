using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Rights;
using Sadie.Game.Rooms.FurnitureItems;
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
    public List<RoomPlayerRight> Rights { get; }
    public IRoomFurnitureItemRepository FurnitureItemRepository { get; }
    public RoomPaintSettings PaintSettings { get; }

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
        List<RoomPlayerRight> rights,
        IRoomFurnitureItemRepository furnitureItemRepository,
        RoomPaintSettings paintSettings) 
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
        Rights = rights;
        FurnitureItemRepository = furnitureItemRepository;
        PaintSettings = paintSettings;
    }
}