using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Bots;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Services;
using Sadie.Database.Models.Players;
using Sadie.Enums.Unsorted;
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
    IRoomWiredService wiredService)
    : RoomUnitData(id,
            room,
            point,
            pointZ,
            directionHead,
            direction,
            tileMapHelperService,
            wiredService),
        IRoomBot
{
    public required PlayerBot Bot { get; init; }

    public async Task RunPeriodicCheckAsync()
    {
        await ProcessGenericChecksAsync();
    }
}