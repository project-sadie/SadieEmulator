using Microsoft.EntityFrameworkCore.Infrastructure;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Rights;

namespace Sadie.Database.Models.Rooms;

public class Room(ILazyLoader lazyLoader)
{
    private Player? _owner;
    private RoomLayout? _layout;
    private RoomSettings? _settings;
    private RoomPaintSettings? _paintSettings;
    private RoomChatSettings? _chatSettings;
    private ICollection<RoomPlayerRight> _playerRights = [];
    
    public int Id { get; init; }
    public required string Name { get; set; }
    public int LayoutId { get; set; }
    
    public RoomLayout? Layout
    {
        get => lazyLoader.Load(this, ref _layout);
        init => _layout = value;
    }
    
    public int OwnerId { get; init; }
    
    public Player? Owner
    {
        get => lazyLoader.Load(this, ref _owner);
        init => _owner = value;
    }
    
    public int MaxUsersAllowed { get; set; }
    public required string Description { get; set; }
    public bool IsMuted { get; init; }
    
    public RoomSettings? Settings
    {
        get => lazyLoader.Load(this, ref _settings);
        init => _settings = value;
    }
    
    public RoomPaintSettings? PaintSettings
    {
        get => lazyLoader.Load(this, ref _paintSettings);
        init => _paintSettings = value;
    }
    
    public RoomChatSettings? ChatSettings
    {
        get => lazyLoader.Load(this, ref _chatSettings);
        init => _chatSettings = value;
    }
    
    public ICollection<RoomPlayerRight> PlayerRights
    {
        get => lazyLoader.Load(this, ref _playerRights);
        init => _playerRights = value;
    }
    
    public ICollection<RoomChatMessage> ChatMessages { get; init; } = [];
    public ICollection<RoomTag> Tags { get; init; } = [];
    public ICollection<PlayerRoomLike> PlayerLikes { get; init; } = [];
    public ICollection<PlayerFurnitureItemPlacementData> FurnitureItems { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public Group? Group { get; init; }
    public RoomDimmerSettings? DimmerSettings { get; set; }
    public ICollection<PlayerRoomBan> PlayerBans { get; init; } = [];
}