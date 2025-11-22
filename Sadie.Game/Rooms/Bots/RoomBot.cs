using System.Drawing;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Bots;
using Sadie.API.Interfaces.Game.Rooms.Mapping;
using Sadie.API.Interfaces.Game.Rooms.Pathfinding;
using Sadie.API.Interfaces.Game.Rooms.Services;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Db.Models.Players;
using Sadie.Game.Rooms.Unit;

namespace Sadie.Game.Rooms.Bots;

public class RoomBot(
    int id,
    IRoomLogic room,
    Point point,
    double pointZ,
    HDirection directionHead,
    HDirection direction,
    IRoomTileMapHelperService tileMapHelperService,
    IRoomWiredService wiredService,
    PlayerBot playerBot,
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
    public required PlayerBot Bot { get; init; } = playerBot;

    public async Task RunPeriodicCheckAsync()
    {
        await ProcessGenericChecksAsync();
    }
}