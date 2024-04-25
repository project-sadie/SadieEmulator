using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Database.Models.Rooms.Rights;

namespace Sadie.Database.Models.Rooms;

public class Room
{
    public int Id { get; init; }
    public string? Name { get; set; }
    public int LayoutId { get; init; }
    public RoomLayout? Layout { get; init; }
    public int OwnerId { get; init; }
    public Player? Owner { get; init; }
    public int MaxUsersAllowed { get; set; }
    public string? Description { get; set; }
    public bool IsMuted { get; init; }
    public RoomSettings? Settings { get; set; }
    public RoomPaintSettings? PaintSettings { get; set; }
    public RoomChatSettings? ChatSettings { get; set; }
    public ICollection<RoomPlayerRight> PlayerRights { get; init; } = [];
    public ICollection<RoomChatMessage> ChatMessages { get; init; } = [];
    public ICollection<RoomTag> Tags { get; init; } = [];
    public ICollection<PlayerRoomLike> PlayerLikes { get; init; } = [];
    public ICollection<RoomFurnitureItem> FurnitureItems { get; init; } = [];
    public DateTime CreatedAt { get; init; }
}