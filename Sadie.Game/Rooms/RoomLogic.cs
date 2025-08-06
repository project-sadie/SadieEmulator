using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Bots;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Users;
using Sadie.Db.Models.Players;
using Sadie.Db.Models.Players.Furniture;
using Sadie.Db.Models.Rooms;
using Sadie.Db.Models.Rooms.Chat;
using Sadie.Db.Models.Rooms.Rights;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Game.Rooms;

public class RoomLogic : Room, IRoomLogic
{
    public RoomLogic(int id,
        string name,
        RoomLayout? layout,
        RoomTileMap tileMap,
        Player? owner,
        string description,
        int maxUsersAllowed,
        bool isMuted,
        IRoomUserRepository userRepository,
        IRoomBotRepository botRepository,
        ICollection<PlayerFurnitureItemPlacementData> furnitureItems,
        RoomSettings? settings,
        RoomChatSettings? chatSettings,
        ICollection<RoomChatMessage> chatMessages,
        ICollection<RoomPlayerRight> playerRights,
        RoomPaintSettings? paintSettings,
        ICollection<RoomTag> tags,
        ICollection<PlayerRoomLike> playerLikes)
    {
        Id = id;
        Name = name;
        Layout = layout;
        Owner = owner;
        MaxUsersAllowed = maxUsersAllowed;
        Description = description;
        IsMuted = isMuted;
        Settings = settings;
        ChatSettings = chatSettings;
        ChatMessages = chatMessages;
        PaintSettings = paintSettings;
        PlayerRights = playerRights;
        Tags = tags;
        PlayerLikes = playerLikes;
        FurnitureItems = furnitureItems;
        TileMap = tileMap;
        UserRepository = userRepository;
        BotRepository = botRepository;
    }
    
    public IRoomTileMap TileMap { get; }
    public IRoomUserRepository UserRepository { get; }
    public IRoomBotRepository BotRepository { get; }

    public async ValueTask DisposeAsync()
    {
    }
}