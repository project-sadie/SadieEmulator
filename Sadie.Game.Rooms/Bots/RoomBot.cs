using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Bots;
using Sadie.Database.Models.Players;

namespace Sadie.Game.Rooms.Bots;

public class RoomBot(IRoomLogic room, Point spawnPoint) : RoomUnitMovementData(room, spawnPoint), IRoomBot
{
    private readonly IRoomLogic _room = room;
    public required int Id { get; init; }
    public required PlayerBot Bot { get; init; }

    public Task RunPeriodicCheckAsync()
    {
        if (NeedsPathCalculated)
        {
            CalculatePath(_room.Settings.WalkDiagonal);
        }

        if (IsWalking)
        {
            ProcessMovement();
        }

        return Task.CompletedTask;
    }
}