using Sadie.Game.Rooms;

namespace SadieEmulator.Tasks.Game.Rooms;

public class ProcessRoomsTask(IRoomRepository roomRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromMilliseconds(500);
    public DateTime LastExecuted { get; set; }

    public async Task ExecuteAsync()
    {
        foreach (var room in roomRepository.GetAllRooms())
        {
            await room.RunPeriodicCheckAsync();
        }
    }
}