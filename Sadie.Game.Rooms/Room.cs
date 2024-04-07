using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Database.Models.Rooms.Rights;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class Room(
    int id,
    string name,
    RoomLayout layout,
    RoomLayoutData layoutData,
    int ownerId,
    string ownerName,
    string description,
    List<string> tags,
    List<PlayerRoomLike> playerLikes,
    int maxUsers,
    bool isMuted,
    IRoomUserRepository userRepository,
    List<RoomFurnitureItem> furnitureItems,
    RoomSettings settings,
    List<RoomChatMessage> chatMessages,
    List<RoomPlayerRight> rights,
    RoomPaintSettings paintSettings)
{
    public int Id { get; } = id;
    public string Name { get; set; } = name;
    public RoomLayout Layout { get; } = layout;
    public int OwnerId { get; } = ownerId;
    public string OwnerName { get; } = ownerName;
    public string Description { get; set; } = description;
    public RoomLayoutData LayoutData { get; } = layoutData;
    public List<string> Tags { get; set; } = tags;
    public int MaxUsers { get; set; } = maxUsers;
    public bool IsMuted { get; } = isMuted;
    public List<PlayerRoomLike> PlayerLikes { get; } = playerLikes;
    public IRoomUserRepository UserRepository { get; } = userRepository;
    public RoomSettings Settings { get; } = settings;
    public List<RoomChatMessage> ChatMessages { get; } = chatMessages;
    public List<RoomPlayerRight> Rights { get; } = rights;
    public List<RoomFurnitureItem> FurnitureItems { get; } = furnitureItems;
    public RoomPaintSettings PaintSettings { get; } = paintSettings;

    public async Task RunPeriodicCheckAsync()
    {
        var users = UserRepository.GetAll();
        
        foreach (var roomUser in users)
        {
            await roomUser.RunPeriodicCheckAsync();
        }

        var statusWriter = new RoomUserStatusWriter(users).GetAllBytes();
        var dataWriter = new RoomUserDataWriter(users).GetAllBytes();

        await UserRepository.BroadcastDataAsync(statusWriter);
        await UserRepository.BroadcastDataAsync(dataWriter);
    }

    public async ValueTask DisposeAsync()
    {
        await UserRepository.DisposeAsync();
    }
}