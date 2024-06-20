using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Bots;
using Sadie.Database.Models.Players;
using Sadie.Enums.Game.Rooms.Unit;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Unit;

namespace Sadie.Game.Rooms.Bots;

public class RoomBot(int id, IRoomLogic room, Point spawnPoint, RoomFurnitureItemInteractorRepository interactorRepository) : RoomUnit(id, RoomUnitType.Bot, room, spawnPoint), IRoomBot
{
    public new int Id { get; } = id;
    public required PlayerBot Bot { get; init; }

    public async Task RunPeriodicCheckAsync()
    {
        if (NeedsPathCalculated)
        {
            CalculatePath();
        }

        if (IsWalking)
        {
            await ProcessMovementAsync();
        }
    }
}