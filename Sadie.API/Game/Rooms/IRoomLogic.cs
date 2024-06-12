using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Rooms.Furniture;

namespace Sadie.API.Game.Rooms;

public interface IRoomLogic
{
    int Id { get; }
    ICollection<RoomFurnitureItem> FurnitureItems { get; }
    IRoomTileMap TileMap { get; }
    IRoomUserRepository UserRepository { get; }
    RoomSettings Settings { get; set; }
}