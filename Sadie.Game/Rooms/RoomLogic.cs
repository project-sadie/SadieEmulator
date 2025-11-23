using Sadie.API.DTOs;
using Sadie.API.DTOs.Player;
using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.DTOs.Rooms;
using Sadie.API.DTOs.Rooms.Chat;
using Sadie.API.DTOs.Rooms.Rights;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Bots;
using Sadie.API.Interfaces.Game.Rooms.Mapping;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Game.Rooms;

public class RoomLogic : IRoomLogic
{
    public RoomLogic(
        int id,
        string name,
        RoomLayoutDto? layout,
        RoomTileMap tileMap,
        PlayerDto? owner,
        string description,
        int maxUsersAllowed,
        bool isMuted,
        IRoomUserRepository userRepository,
        IRoomBotRepository botRepository,
        ICollection<PlayerFurnitureItemPlacementDataDto> furnitureItems,
        RoomSettingsDto? settings,
        RoomChatSettingsDto? chatSettings,
        ICollection<RoomChatMessageDto> chatMessages,
        ICollection<RoomPlayerRightDto> playerRights,
        RoomPaintSettingsDto? paintSettings,
        ICollection<RoomTagDto> tags,
        ICollection<PlayerRoomLikeDto> playerLikes)
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
    public int IdleTicks { get; set; }


    public async ValueTask DisposeAsync()
    {
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public RoomLayoutDto? Layout { get; set; }
    public long OwnerId { get; set; }
    public PlayerDto? Owner { get; set; }
    public int MaxUsersAllowed { get; set; }
    public string Description { get; set; }
    public bool IsMuted { get; set; }
    public RoomSettingsDto? Settings { get; set; }
    public RoomPaintSettingsDto? PaintSettings { get; set; }
    public RoomChatSettingsDto? ChatSettings { get; set; }
    public ICollection<RoomPlayerRightDto> PlayerRights { get; init; }
    public ICollection<RoomChatMessageDto> ChatMessages { get; init; }
    public ICollection<RoomTagDto> Tags { get; init; }
    public ICollection<PlayerRoomLikeDto> PlayerLikes { get; init; }
    public ICollection<PlayerFurnitureItemPlacementDataDto> FurnitureItems { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public GroupDto? Group { get; init; }
    public RoomDimmerSettingsDto? DimmerSettings { get; set; }
}