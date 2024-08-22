using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Rights;

namespace Sadie.Database.Models.Rooms;

public interface IRoom
{
    int Id { get; init; }
    string Name { get; set; }
    int LayoutId { get; set; }
    RoomLayout? Layout { get; set; }
    int OwnerId { get; init; }
    Player? Owner { get; set; }
    int MaxUsersAllowed { get; set; }
    string Description { get; set; }
    bool IsMuted { get; init; }
    RoomSettings? Settings { get; set; }
    RoomPaintSettings? PaintSettings { get; set; }
    RoomChatSettings? ChatSettings { get; set; }
    ICollection<RoomPlayerRight> PlayerRights { get; init; }
    ICollection<RoomChatMessage> ChatMessages { get; init; }
    ICollection<RoomTag> Tags { get; init; }
    ICollection<PlayerRoomLike> PlayerLikes { get; init; }
    ICollection<PlayerFurnitureItemPlacementData> FurnitureItems { get; init; }
    DateTime CreatedAt { get; init; }
    Group? Group { get; init; }
    RoomDimmerSettings? DimmerSettings { get; set; }
    ICollection<PlayerRoomBan> PlayerBans { get; init; }
}