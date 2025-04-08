using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Bots;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Pathfinding;
using Sadie.API.Game.Rooms.Services;
using Sadie.Database.Models.Players;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms.Unit;

namespace Sadie.Game.Rooms.Bots;

public class RoomBot(
    IRoomLogic room,
    Point point,
    double pointZ,
    HDirection directionHead,
    HDirection direction,
    IRoomTileMapHelperService tileMapHelperService,
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
    public required PlayerBot Bot { get; init; }

    public async Task RunPeriodicCheckAsync()
    {
        await ProcessGenericChecksAsync();
    }
}