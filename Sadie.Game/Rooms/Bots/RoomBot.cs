using System.Drawing;
using Sadie.API.DTOs.Player;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Bots;
using Sadie.API.Interfaces.Game.Rooms.Mapping;
using Sadie.API.Interfaces.Game.Rooms.Pathfinding;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Game.Rooms.Unit;

namespace Sadie.Game.Rooms.Bots;

public class RoomBot(
    IRoomLogic room,
    Point point,
    double pointZ,
    HDirection directionHead,
    HDirection direction,
    IRoomTileMapHelperService tileMapHelperService,
    PlayerBotDto playerBot,
    IRoomPathFinderHelperService pathFinderHelperService)
    : RoomUnitData(room,
            point,
            pointZ,
            directionHead,
            direction,
            tileMapHelperService,
            pathFinderHelperService),
        IRoomBot
{
    public required PlayerBotDto Bot { get; init; } = playerBot;

    public async Task RunPeriodicCheckAsync()
    {
        await ProcessGenericChecksAsync();
    }
}