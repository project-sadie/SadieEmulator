using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Database.Models.Rooms.Rights;

namespace Sadie.Database.Models.Rooms;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int LayoutId { get; set; }
    public RoomLayout Layout { get; set; }
    public int OwnerId { get; set; }
    public Player Owner { get; set; }
    public int MaxUsersAllowed { get; set; }
    public string Description { get; set; }
    public bool IsMuted { get; set; }
    public RoomSettings Settings { get; set; }
    public RoomPaintSettings PaintSettings { get; set; }
    public ICollection<RoomPlayerRight> PlayerRights { get; set; } = [];
    public ICollection<RoomChatMessage> ChatMessages { get; set; } = [];
    public ICollection<RoomTag> Tags { get; set; } = [];
    public ICollection<PlayerRoomLike> PlayerLikes { get; set; } = [];
    public ICollection<RoomFurnitureItem> FurnitureItems { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}