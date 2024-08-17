using Sadie.API.Game.Rooms.Bots;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Rights;

namespace Sadie.API.Game.Rooms;

public interface IRoomLogic : IAsyncDisposable
{
    int Id { get; }
    string Name { get; set; }
    int LayoutId { get; set; }
    RoomLayout? Layout { get; set; }
    int OwnerId { get; }
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
    ICollection<PlayerFurnitureItemPlacementData> FurnitureItems { get; }
    RoomDimmerSettings? DimmerSettings { get; set; }
    ICollection<PlayerRoomBan> PlayerBans { get; init; }
    IRoomTileMap TileMap { get; }
    IRoomUserRepository UserRepository { get; }
    IRoomBotRepository BotRepository { get; }
}