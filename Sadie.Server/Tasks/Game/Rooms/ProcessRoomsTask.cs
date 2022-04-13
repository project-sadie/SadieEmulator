using Sadie.Game.Rooms;

namespace SadieEmulator.Tasks.Game.Rooms;

public class ProcessRoomsTask : IServerTask
{
    public string Name => "ProcessRooms";
    public TimeSpan PeriodicInterval => TimeSpan.FromMilliseconds(500);
    public DateTime LastExecuted { get; set; }
    
    private readonly IRoomRepository _roomRepository;
    
    public ProcessRoomsTask(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    public async Task ExecuteAsync()
    {
        foreach (var room in _roomRepository.GetAllRooms())
        {
            await room.RunPeriodicCheckAsync();
        }
    }
}