using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Database.Models.Rooms.Rights;
using Sadie.Game.Rooms.Tiles;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class RoomLogic : Room
{
    public RoomLogic(int id,
        string name,
        RoomLayout layout,
        RoomTileMap tileMap,
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
        TileMap = tileMap;
        UserRepository = userRepository;
    }
    
    public RoomTileMap TileMap { get; }
    public IRoomUserRepository UserRepository { get; }

    public async ValueTask DisposeAsync()
    {
        await UserRepository.DisposeAsync();
    }
}