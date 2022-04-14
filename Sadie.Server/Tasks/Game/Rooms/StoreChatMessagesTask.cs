using Sadie.Game.Rooms;

namespace SadieEmulator.Tasks.Game.Rooms;

public class StoreChatMessagesTask : IServerTask
{
    public string Name => "StoreChatMessages";
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(30);
    public DateTime LastExecuted { get; set; }
    
    private readonly IRoomRepository _roomRepository;
    
    public StoreChatMessagesTask(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    public async Task ExecuteAsync()
    {
        foreach (var room in _roomRepository.GetAllRooms())
        {
            if (room.ChatMessages.Count <= 0)
            {
                continue;
            }

            try
            {
                await _roomRepository.CreateChatMessages(room.ChatMessages);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("We saved " + room.ChatMessages.Count + " messages");
            room.ChatMessages.Clear();
        }
    }
}