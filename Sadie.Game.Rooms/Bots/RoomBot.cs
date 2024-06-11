using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Bots;
using Sadie.Database.Models.Players;

namespace Sadie.Game.Rooms.Bots;

public class RoomBot(IRoomLogic room) : RoomUnitMovementData(room), IRoomBot
{
    public required int Id { get; init; }
    public required PlayerBot Bot { get; init; }
    public IRoomLogic Room { get; } = room;

    public async Task RunPeriodicCheckAsync()
    {
        if (NextPoint != null)
        {
            Room.TileMap.BotMap[Point].Remove(this);
            Room.TileMap.AddBotToMap(NextPoint.Value, this);
            
            Point = NextPoint.Value;
            NextPoint = null;
        }

        if (NeedsPathCalculated)
        {
            CalculatePath(room.Settings.WalkDiagonal);
        }

        if (IsWalking)
        {
            ProcessMovement();
        }
    }
}