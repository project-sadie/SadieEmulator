using Sadie.API.Interfaces.Game.Rooms;

namespace SadieEmulator.Tasks.Game.Rooms;

public class ProcessRoomUnitsTask(IRoomRepository roomRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromMilliseconds(500);
    public DateTime LastExecuted { get; set; }

    private int _isRunning;
    
    public async Task ExecuteAsync()
    {
        if (Interlocked.Exchange(ref _isRunning, 1) == 1)
        {
            return;
        }

        try
        {
            await Parallel.ForEachAsync(
                roomRepository.GetAllRooms(),
                new ParallelOptions { MaxDegreeOfParallelism = 4 },
                RunPeriodicChecksForRoomAsync
            );
        }
        finally
        {
            _isRunning = 0;
        }
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