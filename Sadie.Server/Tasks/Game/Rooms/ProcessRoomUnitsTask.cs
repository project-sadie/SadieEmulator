using Sadie.API.Game.Rooms;
using Sadie.Game.Rooms;

namespace SadieEmulator.Tasks.Game.Rooms;

public class ProcessRoomUnitsTask(RoomRepository roomRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromMilliseconds(500);
    public DateTime LastExecuted { get; set; }

    public async Task ExecuteAsync()
    {
        await Parallel.ForEachAsync(roomRepository.GetAllRooms(), RunPeriodicChecksForRoomAsync);
    }

    private static async ValueTask RunPeriodicChecksForRoomAsync(IRoomLogic? room, CancellationToken ctx)
    {
        if (room == null)
        {
            return;
        }

        await room.BotRepository.RunPeriodicCheckAsync();
        await room.UserRepository.RunPeriodicCheckAsync();
    }
}