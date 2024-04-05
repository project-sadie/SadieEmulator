using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Rights;
using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public interface IRoomData
{
    int Id { get; }
    string Name { get; set; }
    RoomLayout Layout { get; }
    int OwnerId { get; }
    string OwnerName { get; }
    string Description { get; set; }
    int Score { get; }
    List<string> Tags { get; set; }
    int MaxUsers { get; set; }
    bool Muted { get; }
    IRoomUserRepository UserRepository { get; }
    RoomSettings Settings { get; }
    List<RoomChatMessage> ChatMessages { get; }
    public List<RoomPlayerRight> Rights { get; }
    public IRoomFurnitureItemRepository FurnitureItemRepository { get; }
    public RoomPaintSettings PaintSettings { get; }
}