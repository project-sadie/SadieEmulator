using Sadie.API.Game.Rooms.Bots;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms;

namespace Sadie.API.Game.Rooms;

public interface IRoomLogic
{
    int Id { get; }
    ICollection<PlayerFurnitureItemPlacementData> FurnitureItems { get; }
    IRoomTileMap TileMap { get; }
    IRoomUserRepository UserRepository { get; }
    IRoomBotRepository BotRepository { get; }
    RoomSettings? Settings { get; }
    RoomDimmerSettings? DimmerSettings { get; set; }
}