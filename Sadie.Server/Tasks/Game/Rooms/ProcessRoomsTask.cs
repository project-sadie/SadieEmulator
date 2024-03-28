using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms;

namespace SadieEmulator.Tasks.Game.Rooms;

public class ProcessRoomsTask(ILogger<ProcessRoomsTask> logger, IRoomRepository roomRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromMilliseconds(500);
    public DateTime LastExecuted { get; set; }

    public async Task ExecuteAsync()
    { 
        logger.LogWarning("processing room");
        foreach (var room in roomRepository.GetAllRooms())
        {
            await room.RunPeriodicCheckAsync();
        }
    }
}