using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Rights;

namespace Sadie.Database.Models.Rooms;

public class Room 
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public int LayoutId { get; set; }
    public RoomLayout? Layout { get; set; }
    public int OwnerId { get; init; }
    public Player? Owner { get; set; }
    public int MaxUsersAllowed { get; set; }
    public required string Description { get; set; }
    public bool IsMuted { get; init; }
    public RoomSettings? Settings { get; set; }
    public RoomPaintSettings? PaintSettings { get; set; }
    public RoomChatSettings? ChatSettings { get; set; }
    public ICollection<RoomPlayerRight> PlayerRights { get; init; } = [];
    public ICollection<RoomChatMessage> ChatMessages { get; init; } = [];
    public ICollection<RoomTag> Tags { get; init; } = [];
    public ICollection<PlayerRoomLike> PlayerLikes { get; init; } = [];
    public ICollection<PlayerFurnitureItemPlacementData> FurnitureItems { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public Group? Group { get; init; }
    public RoomDimmerSettings? DimmerSettings { get; set; }
    public ICollection<PlayerRoomBan> PlayerBans { get; init; } = [];
}