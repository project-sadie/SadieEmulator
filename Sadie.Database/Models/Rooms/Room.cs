using Microsoft.EntityFrameworkCore.Infrastructure;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Rights;

namespace Sadie.Database.Models.Rooms;

public class Room : IRoom
{
    private readonly ILazyLoader _lazyLoader;
    private Player? _owner;
    private RoomLayout? _layout;
    private RoomPaintSettings? _paintSettings;
    private RoomChatSettings? _chatSettings;
    private ICollection<RoomPlayerRight> _playerRights = [];
    private ICollection<PlayerFurnitureItemPlacementData> _furnitureItems = [];
    
    public Room()
    {
    }
    
    public int Id { get; init; }
    public required string Name { get; set; }
    public int LayoutId { get; set; }
    
    public RoomLayout? Layout
    {
        get => _lazyLoader.Load(this, ref _layout);
        set => _layout = value;
    }
    
    public long OwnerId { get; init; }
    
    public Player? Owner
    {
        get => _lazyLoader.Load(this, ref _owner);
        set => _owner = value;
    }
    
    public int MaxUsersAllowed { get; set; }
    public required string Description { get; set; }
    public bool IsMuted { get; init; }
    
    public RoomSettings Settings { get; set; }
    
    public RoomPaintSettings? PaintSettings
    {
        get => _lazyLoader.Load(this, ref _paintSettings);
        set => _paintSettings = value;
    }
    
    public RoomChatSettings? ChatSettings
    {
        get => _lazyLoader.Load(this, ref _chatSettings);
        set => _chatSettings = value;
    }
    
    public ICollection<RoomPlayerRight> PlayerRights
    {
        get => _lazyLoader.Load(this, ref _playerRights);
        init => _playerRights = value;
    }
    
    public ICollection<RoomChatMessage> ChatMessages { get; init; } = [];
    public ICollection<RoomTag> Tags { get; init; } = [];
    public ICollection<PlayerRoomLike> PlayerLikes { get; init; } = [];
    
    public ICollection<PlayerFurnitureItemPlacementData> FurnitureItems
    {
        get => _lazyLoader.Load(this, ref _furnitureItems);
        init => _furnitureItems = value;
    }
    
    public DateTime CreatedAt { get; init; }
    public Group? Group { get; init; }
    public RoomDimmerSettings? DimmerSettings { get; set; }
    public ICollection<PlayerRoomBan> PlayerBans { get; init; } = [];
}