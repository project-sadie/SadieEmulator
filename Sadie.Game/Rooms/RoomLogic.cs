using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Bots;
using Sadie.API.Interfaces.Game.Rooms.Mapping;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Db.Models.Rooms;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Game.Rooms;

public class RoomLogic(
    RoomDto room,
    RoomTileMap tileMap,
    IRoomUserRepository userRepository,
    IRoomBotRepository botRepository) : Room, IRoomLogic
{
    public RoomDto Room { get; } = room;
    public IRoomTileMap TileMap { get; } = tileMap;
    public IRoomUserRepository UserRepository { get; } = userRepository;
    public IRoomBotRepository BotRepository { get; } = botRepository;

    public async ValueTask DisposeAsync()
    {
    }
}