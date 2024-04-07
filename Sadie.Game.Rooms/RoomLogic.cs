using Sadie.Database.Models;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Database.Models.Rooms.Rights;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class RoomLogic : Room
{
    public RoomLogic(int id,
        string name,
        RoomLayout layout,
        RoomLayoutData layoutData,
        Player owner,
        string description,
        int maxUsersAllowed,
        bool isMuted,
        IRoomUserRepository userRepository,
        List<RoomFurnitureItem> furnitureItems,
        RoomSettings settings,
        List<RoomChatMessage> chatMessages,
        List<RoomPlayerRight> playerRights,
        RoomPaintSettings paintSettings,
        List<RoomTag> tags,
        List<PlayerRoomLike> playerLikes)
    {
        Id = id;
        Name = name;
        Layout = layout;
        Owner = owner;
        MaxUsersAllowed = maxUsersAllowed;
        Description = description;
        IsMuted = isMuted;
        Settings = settings;
        ChatMessages = chatMessages;
        PaintSettings = paintSettings;
        PlayerRights = playerRights;
        Tags = tags;
        PlayerLikes = playerLikes;
        FurnitureItems = furnitureItems;
        LayoutData = layoutData;
        UserRepository = userRepository;
    }
    
    public RoomLayoutData LayoutData { get; }
    public IRoomUserRepository UserRepository { get; }

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